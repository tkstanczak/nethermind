/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using DotNetty.Buffers;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Test.Builders;
using Nethermind.Logging;
using Nethermind.Network.P2P.Subprotocols.Eth;
using Nethermind.Network.Rlpx;
using Org.BouncyCastle.Crypto.Digests;

namespace Nethermind.Network.Benchmarks
{
    [MemoryDiagnoser]
    [CoreJob(baseline: true)]
    public  class OutFlow
    {
        private NewBlockMessageSerializer _newBlockMessageSerializer;
        private Block _block;
        private TestSplitter _splitter;
        private TestEncoder _encoder;
        private TestSnappy _snappyEncoder;
        private IByteBuffer _byteBuffer;

        [GlobalSetup]
        public void Setup()
        {
            PublicKey publicKey = new PublicKey(
                "000102030405060708090A0B0C0D0E0F" +
                "101112131415161718191A1B1C1D1E1F" +
                "202122232425262728292A2B2C2D2E2F" +
                "303132333435363738393A3B3C3D3E3F");
            EncryptionSecrets secrets = new EncryptionSecrets();
            secrets.AesSecret = Keccak.EmptyTreeHash.Bytes;
            secrets.MacSecret = Keccak.OfAnEmptySequenceRlp.Bytes;
            secrets.Token = Keccak.OfAnEmptyString.Bytes;
            secrets.EgressMac = new KeccakDigest(256);
            secrets.IngressMac = new KeccakDigest(256);

            FrameCipher frameCipher = new FrameCipher(secrets.AesSecret);
            FrameMacProcessor frameMacProcessor = new FrameMacProcessor(publicKey, secrets);
            _encoder = new TestEncoder(frameCipher, frameMacProcessor, LimboTraceLogger.Instance);
            _splitter = new TestSplitter();
            _splitter.DisableFraming();
            _snappyEncoder = new TestSnappy();
            _byteBuffer = new MockBuffer();
            Transaction a = Build.A.Transaction.TestObject;
            Transaction b = Build.A.Transaction.TestObject;
            _block = Build.A.Block.WithTransactions(a, b).TestObject;
            _newBlockMessageSerializer = new NewBlockMessageSerializer();
        }

        private class TestEncoder : Rlpx.NettyFrameEncoder
        {
            public void Encode(byte[] message, IByteBuffer buffer)
            {
                base.Encode(null, message, buffer);
            }

            public TestEncoder(IFrameCipher frameCipher, IFrameMacProcessor frameMacProcessor, ILogger logger)
                : base(frameCipher, frameMacProcessor, logger)
            {
            }
        }

        private class TestSplitter : Rlpx.NettyPacketSplitter
        {
            public void Encode(Packet message, List<object> output)
            {
                base.Encode(null, message, output);
            }
        }

        public class TestSnappy : SnappyEncoder
        {
            public TestSnappy()
                : base(NullLogger.Instance)
            {
            }

            public Packet TestEncode(byte[] input)
            {
                List<object> result = new List<object>();
                Encode(null, new Packet(input), result);
                return (Packet) result[0];
            }
        }

//        [Benchmark]
//        public bool Improved()
//        {
//            throw new NotImplementedException();
//        }

        [Benchmark(Baseline = true)]
        public void Current()
        {
            NewBlockMessage newBlockMessage = new NewBlockMessage();
            newBlockMessage.Block = _block;
            byte[] message = _newBlockMessageSerializer.Serialize(newBlockMessage);
            Packet packet = new Packet("eth", 1, message);
            Packet ensnapped = _snappyEncoder.TestEncode(packet.Data);
            List<object> output = new List<object>();
            _splitter.Encode(ensnapped, output);
            _encoder.Encode((byte[]) output[0], _byteBuffer);
        }

        [Benchmark]
        public void Current_no_snappy()
        {
            NewBlockMessage newBlockMessage = new NewBlockMessage();
            newBlockMessage.Block = _block;
            byte[] message = _newBlockMessageSerializer.Serialize(newBlockMessage);
            Packet packet = new Packet("eth", 1, message);
            List<object> output = new List<object>();
            _splitter.Encode(packet, output);
            _encoder.Encode((byte[]) output[0], _byteBuffer);
        }

        [Benchmark]
        public void Current_no_encryption()
        {
            NewBlockMessage newBlockMessage = new NewBlockMessage();
            newBlockMessage.Block = _block;
            byte[] message = _newBlockMessageSerializer.Serialize(newBlockMessage);
            Packet packet = new Packet("eth", 1, message);
            Packet ensnapped = _snappyEncoder.TestEncode(packet.Data);
            List<object> output = new List<object>();
            _splitter.Encode(ensnapped, output);
            _encoder.Encode((byte[]) output[0], _byteBuffer);
        }

        [Benchmark]
        public void Current_no_encoder()
        {
            NewBlockMessage newBlockMessage = new NewBlockMessage();
            newBlockMessage.Block = _block;
            byte[] message = _newBlockMessageSerializer.Serialize(newBlockMessage);
            Packet packet = new Packet("eth", 1, message);
            Packet ensnapped = _snappyEncoder.TestEncode(packet.Data);
            List<object> output = new List<object>();
            _splitter.Encode(ensnapped, output);
        }
    }
}