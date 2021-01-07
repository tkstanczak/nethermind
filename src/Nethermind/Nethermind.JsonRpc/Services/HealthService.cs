﻿//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
// 

using System.Collections.Generic;
using System.Linq;
using Nethermind.Blockchain.Processing;
using Nethermind.Consensus;
using Nethermind.JsonRpc.Modules.Eth;
using Nethermind.JsonRpc.Modules.Net;

namespace Nethermind.JsonRpc.Services
{
    public class CheckHealthResult
    {
        public bool Healthy { get; set; }

        public string Message { get; set; }

        public string LongMessage { get; set; }
    }

    public class HealthService : IHealthService
    {
        private readonly IEthModule _ethModule;
        private readonly INetModule _netModule;
        private readonly IBlockchainProcessor _blockchainProcessor;
        private readonly IBlockProducer _blockProducer;
        private readonly bool _isMining;

        public HealthService(IEthModule ethModule, INetModule netModule, IBlockchainProcessor blockchainProcessor, IBlockProducer blockProducer, bool isMining)
        {
            _ethModule = ethModule;
            _netModule = netModule;
            _isMining = isMining;
            _blockchainProcessor = blockchainProcessor;
            _blockProducer = blockProducer;
        }

        public CheckHealthResult CheckHealth()
        {
            List<(string Message, string LongMessage)> messages = new List<(string Message, string LongMessage)>();
            bool healthy = false;
            long netPeerCount = (long)_netModule.net_peerCount().GetData();
            SyncingResult ethSyncing = (SyncingResult)_ethModule.eth_syncing().GetData();
            
            if (_isMining == false && ethSyncing.IsSyncing)
            {
                healthy = false;
                messages.Add(("Still syncing", $"The node is still syncing, CurrentBlock: {ethSyncing.CurrentBlock}, HighestBlock: {ethSyncing.HighestBlock}, Peers: {netPeerCount}"));
            }
            else if (_isMining == false && ethSyncing.IsSyncing == false)
            {
                bool peers = CheckPeers(messages, netPeerCount);
                bool processing = IsProcessingBlocks(messages, _blockchainProcessor.IsProcessingBlocks);
                healthy = peers && processing;
            }
            else if (_isMining && ethSyncing.IsSyncing)
            {
                healthy = CheckPeers(messages, netPeerCount);
            }
            else if (_isMining && ethSyncing.IsSyncing == false)
            {
                bool peers = CheckPeers(messages, netPeerCount);
                bool processing = IsProcessingBlocks(messages, _blockchainProcessor.IsProcessingBlocks);
                bool producing = IsProducingBlocks(messages, _blockProducer.IsProducingBlocks);
                healthy = peers && processing && producing;
            }
            
            return new CheckHealthResult()
            {
                Healthy = healthy, 
                Message = FormatMessages(messages.Select(x => x.Message)),
                LongMessage = FormatMessages(messages.Select(x => x.LongMessage))
            };
        }

        private static bool CheckPeers(ICollection<(string Description, string LongDescription)> errors, long netPeerCount)
        {
            bool hasPeers = netPeerCount > 0;
            if (hasPeers == false)
            {
                errors.Add(("Node is not connected to any peers", "Node is not connected to any peers"));  
            }

            return hasPeers;
        }
        
        private static bool IsProducingBlocks(ICollection<(string Description, string LongDescription)> errors, bool producingBlocks)
        {
            if (producingBlocks == false)
            {
                errors.Add(("Stopped producing blocks", "The node stopped producing blocks"));  
            }

            return producingBlocks;
        }
        
        private static bool IsProcessingBlocks(ICollection<(string Description, string LongDescription)> errors, bool processingBlocks)
        {
            if (processingBlocks == false)
            {
                errors.Add(("Stopped processing blocks", "The node stopped processing blocks"));  
            }

            return processingBlocks;
        }

        private static string FormatMessages(IEnumerable<string> messages)
        {
            return messages.Any() ? string.Join(". ", messages) + "." : string.Empty;
        }
    }
}
