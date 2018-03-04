// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Problemator.Core.Messages
{
    public class LocationChangedMessage
    {
        public string NewLocation { get; }

        public LocationChangedMessage(string newLocation)
        {
            NewLocation = newLocation;
        }
    }
}