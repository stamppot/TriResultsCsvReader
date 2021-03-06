﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConfigReader;
using FileAppServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriResultsCsvReader;
using TriResultsCsvReader.StandardizeHeaders;

namespace ResultsCsvReader.Test
{
    [TestClass]
    public class FilteredResultsReaderCsvTest
    {
        private HeaderStandardizer _resultsReaderCsv;
        private IWhitelistFilter _membersWhitelist;
        private IWhitelistFilter _clubWhitelist;

        public FilteredResultsReaderCsvTest()
        {
            IColumnConfigProvider columnConfigProvider = new ColumnConfigProvider("column_config.xml");
            _resultsReaderCsv = new HeaderStandardizer(columnConfigProvider);
            _membersWhitelist = new WhitelistFilter(new List<string> { "van der Maas", "Rep", "Terwisscha Van Scheltinga", "Symen de Jong", "Niels Grote Beverborg", "Bram Smit", "Thijs Wiggers", "Erik-Jan de Groot", "Margot Reinders", "Fenna Heijnen", "Jorinde van der Laan", "Anne Hobbelt" });
            _clubWhitelist = new WhitelistFilter(new List<string> { "Tritanium", "Groningen", "Triteam Groningen" });
        }

        public IEnumerable<string> ReadFileResults(string file)
        {
            return new CsvResultsReader().ReadRaceRows(file);
        }

        [TestMethod]
        public void ReadResults_FilterName_LeiderdorpSprint_None_Test()
        {
            var csvFile = "files/Leiderdorp_sprint.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines,csvFile); //, ((row) => _membersWhitelist.ExactMatch(row.Naam)));

            Assert.IsTrue(!results.Any());
        }

        [TestMethod]
        public void ReadResults_FilterName_LeiderdorpSprintIndi_Some_Test()
        {
            var csvFile = "files/Leiderdorp_sprint_indi.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines,csvFile); //, ((row) => _membersWhitelist.ExactMatch(row.Naam)));

            Assert.IsTrue(results.Any());

            Assert.IsTrue(results.Any(r => r.Naam == "Bram Smit"));
        }

        [TestMethod]
        public void ReadResults_Leiderdorp1D_ClubFilter_Test()
        {
            var csvFile = "files/Leiderdorp_1D.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _clubWhitelist.StartsWithMatch(row.Club)));

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_OuderkerkadAmstel_sprint_CityFilter_Test()
        {
            var csvFile = "files/ouderkerkadamstel_sprint_m.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _clubWhitelist.StartsWithMatch(row.City)));

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_UTT_ochtend_overall_Test()
        {
            var csvFile = "files/utt_ochtend_overall.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _clubWhitelist.ContainsMatch(row.Club)));

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_UTT_middag_overall_Test()
        {
            var csvFile = "files/utt_middag_overall.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _clubWhitelist.ContainsMatch(row.Club)));

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_almere_duin_team_Test()
        {
            var csvFile = "files/duin_almere_team.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _membersWhitelist.ContainsMatch(row.Naam)));

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_ouderkerkadamstel_sprint_m_Test()
        {
            var csvFile = "files/ouderkerkadamstel_sprint_m.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_Rotterdam_RBR_overall_Test()
        {
            var csvFile = "files/Rotterdam_RBR_overall.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ReadResults_Tri_Schagen_indi_Test()
        {
            var csvFile = "files/Tri_Schagen_indi.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile);

            Assert.IsTrue(results.Any());
        }


        [TestMethod]
        public void ReadResults_Zeewolde_OD_2D_M_Test()
        {
            var csvFile = "files/Zeewolde_OD_2D_m.csv";
            var lines = ReadFileResults(csvFile);
            var results = _resultsReaderCsv.StandardizeHeaders(lines, csvFile); //, ((row) => _clubWhitelist.ContainsMatch(row.Club))); // binding whitelist to lambda

            Assert.IsTrue(results.Any());
        }
    }
}