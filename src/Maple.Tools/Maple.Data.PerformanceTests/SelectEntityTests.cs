using BenchmarkDotNet.Attributes;
using Maple.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Data.PerformanceTests
{    
    //[ClrJob(true), CoreJob, MonoJob, CoreRtJob]
    [CoreJob]
    [RPlotExporter, RankColumn]
    public class SelectEntityTests 
    {
        protected MapleDataTest mapleDataTest = null;
        protected MapleRepositoryDataTest mapleRepositoryDataTest = null;
        protected DapperDataTest dapperDataTest = null;
        protected EFDataTest efDataTest = null;
        protected IdWorker idWorker = null;

        [GlobalSetup]
        public void Setup()
        {
            this.mapleDataTest = new MapleDataTest();
            this.dapperDataTest = new DapperDataTest();
            this.efDataTest = new EFDataTest();
            this.idWorker = new IdWorker(0);
            this.mapleRepositoryDataTest = new MapleRepositoryDataTest();


            this.mapleDataTest.SelectAll();
            this.dapperDataTest.SelectAll();
            this.efDataTest.SelectAll();
            this.mapleRepositoryDataTest.SelectAll();
        }



        [Benchmark]
        public void MapleSelect()
        {
            this.mapleDataTest.SelectAll();
        }
        [Benchmark]
        public void MapleRepositorySelect()
        {
            this.mapleRepositoryDataTest.SelectAll();
        }
        [Benchmark]
        public void DapperSelect()
        {
            this.dapperDataTest.SelectAll();
        }

        [Benchmark]
        public void EfSelect()
        {
            this.efDataTest.SelectAll();
        }

    }
}
