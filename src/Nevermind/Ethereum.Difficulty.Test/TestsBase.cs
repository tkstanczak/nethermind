﻿/*
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Ethereum.Test.Base;
using Nevermind.Blockchain.Difficulty;
using Nevermind.Core;
using Nevermind.Core.Crypto;
using Nevermind.Core.Extensions;
using Nevermind.Core.Potocol;
using NUnit.Framework;

namespace Ethereum.Difficulty.Test
{
    public abstract class TestsBase
    {
        public static IEnumerable<DifficultyTest> Load(string fileName)
        {
            return TestLoader.LoadFromFile<Dictionary<string, DifficultyTestJson>, DifficultyTest>(
                fileName,
                t => t.Select(dtj => ToTest(fileName, dtj.Key, dtj.Value)));
        }

        public static IEnumerable<DifficultyTest> LoadHex(string fileName)
        {
            Stopwatch watch = new Stopwatch();
            IEnumerable<DifficultyTest> tests = TestLoader.LoadFromFile<Dictionary<string, DifficultyTestHexJson>, DifficultyTest>(
                fileName,
                t => t.Select(dtj => ToTest(fileName, dtj.Key, dtj.Value))).ToList();
            watch.Stop();
            return tests;
        }

        protected static DifficultyTest ToTest(string fileName, string name, DifficultyTestJson json)
        {
            return new DifficultyTest(
                fileName,
                name,
                json.ParentTimestamp,
                json.ParentDifficulty,
                json.CurrentTimestamp,
                (ulong)json.CurrentBlockNumber,
                json.CurrentDifficulty,
                false);
        }

        private static BigInteger ToBigInteger(string hex)
        {
            hex = hex.Replace("0x", "0");
            return BigInteger.Parse(hex, NumberStyles.HexNumber);
        }

        private static ulong ToUlong(string hex)
        {
            byte[] bytes = Hex.ToBytes(hex);
            return bytes.ToUInt64();
        }

        protected static DifficultyTest ToTest(string fileName, string name, DifficultyTestHexJson json)
        {
            Keccak noUnclesHash = Keccak.OfAnEmptySequenceRlp;

            return new DifficultyTest(
                fileName,
                name,
                ToBigInteger(json.ParentTimestamp),
                ToBigInteger(json.ParentDifficulty),
                ToBigInteger(json.CurrentTimestamp),
                ToUlong(json.CurrentBlockNumber),
                ToBigInteger(json.CurrentDifficulty),
                !string.IsNullOrWhiteSpace(json.ParentUncles) && new Keccak(json.ParentUncles) != noUnclesHash);
        }

        protected void RunTest(DifficultyTest test, EthereumNetwork network)
        {
            ProtocolSpecificationProvider specProvider = new ProtocolSpecificationProvider();
            IDifficultyCalculator calculator = new ProtocolBasedDifficultyCalculator(specProvider.GetSpec(network, test.CurrentBlockNumber));

            BigInteger difficulty = calculator.Calculate(
                test.ParentDifficulty,
                test.ParentTimestamp,
                test.CurrentTimestamp,
                test.CurrentBlockNumber,
                test.ParentHasUncles);

            Assert.AreEqual(test.CurrentDifficulty, difficulty, test.Name);
        }
    }
}