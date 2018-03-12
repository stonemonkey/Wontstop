// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient.Parsers;
using MvvmToolkit;
using MvvmToolkit.Messages;
using MvvmToolkit.Utils;
using Problemator.Core.Dtos;
using Problemator.Core.Utils;
using System;
using System.Threading.Tasks;

namespace Problemator.Core.Models
{
    public class UserContext
    {
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public UserContext(
            IStorageService storageService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        private UserIdentity _userIdentity;

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            email.ValidateNotNullEmptyWhiteSpace(nameof(email));
            password.ValidateNotNullEmptyWhiteSpace(nameof(password));

            var succeeded = false;

            var response = (await _requestsFactory.CreateLoginRequest(email, password)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        _userIdentity = p.To<UserIdentity>();
                        SaveUserIdentity(_userIdentity);
                        succeeded = true;
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);

            return succeeded;
        }

        public async Task DeauthenticateAsync()
        {
            await _requestsFactory.CreateLogoutRequest()
                .RunAsync<StringParser>();

            _userIdentity = null;
            _requestsFactory.ClearUserContext();
            _storageService.DeleteLocal(Settings.ContextKey);
        }

        public UserIdentity GetUserIdentity()
        {
            LoadUserIdentity();

            return new UserIdentity(_userIdentity);
        }

        public void UpdateUserIdentity(string gymId, GymChangeConfirmation confirmation)
        {
            gymId.ValidateNotNullEmptyWhiteSpace(nameof(gymId));
            confirmation.ValidateNotNull(nameof(confirmation));

            LoadUserIdentity();

            _userIdentity.GymId = gymId;
            _userIdentity.Jwt = confirmation.Jwt;
            _userIdentity.Message = confirmation.Message;

            SaveUserIdentity(_userIdentity);
        }

        private void LoadUserIdentity()
        {
            if (_userIdentity == null)
            {
                _userIdentity = _storageService.ReadLocal<UserIdentity>(Settings.ContextKey);
            }

            if (_userIdentity == null)
            {
                throw new InvalidOperationException("Unauthenticated user!");
            }
        }

        private void SaveUserIdentity(UserIdentity identity)
        {
            _storageService.SaveLocal(Settings.ContextKey, identity);

            _requestsFactory.SetUserContext(_userIdentity);
        }
    }
}
