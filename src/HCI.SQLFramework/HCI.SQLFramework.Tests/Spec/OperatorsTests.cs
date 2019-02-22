using HCI.SQLFramework.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HCI.SQLFramework.Tests.Spec
{
    public class OperatorsTests
    {
        [Fact]
        public void AndTest()
        {
            var spec = new Specification();
           var query = spec.Where<DTO.Archive>(x => x.Id > 0);
        }
        [Fact]
        public void OrTest()
        {

        }
        [Fact]
        public void EqualTest()
        {

        }
        [Fact]
        public void NotEqualTest()
        {

        }
        [Fact]
        public void InTest()
        {

        }
        [Fact]
        public void NotInTest()
        {

        }       
        [Fact]
        public void IsNullTest()
        {

        }
        [Fact]
        public void IsNotNullTest()
        {

        }
    }
}
