using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.ControlPlane.GraphAlgorithm;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class DijkstraRouteEngineTests {

        [TestMethod]
        public void Calculate_A_to_D_given_AB_BC_CD__should_be__ABCD() {
            var results = Engine.CalculateShortestPathBetween(
                "A",
                "D",
                new[] {
                new Path<string> { Source = "A", Destination = "B", Cost = 3 },
                new Path<string> { Source = "B", Destination = "C", Cost = 3 },
                new Path<string> { Source = "C", Destination = "D", Cost = 3 }
                });

            results.Sum(r => r.Cost).Should().Be(9);
            results.Count.Should().Be(3);

            results.First().Cost.Should().Be(3);
            results.First().Source.Should().Be("A");
            results.First().Destination.Should().Be("B");

            results.Skip(1).First().Cost.Should().Be(3);
            results.Skip(1).First().Source.Should().Be("B");
            results.Skip(1).First().Destination.Should().Be("C");

            results.Skip(2).First().Cost.Should().Be(3);
            results.Skip(2).First().Source.Should().Be("C");
            results.Skip(2).First().Destination.Should().Be("D");
        }

        [TestMethod]
        public void Calculate_A_to_D_given_AB_BC_CD_DE__should_be__ABCD() {
            var results = Engine.CalculateShortestPathBetween(
                "A",
                "D",
                new[] {
                new Path<string> { Source = "A", Destination = "B", Cost = 3 },
                new Path<string> { Source = "B", Destination = "C", Cost = 3 },
                new Path<string> { Source = "C", Destination = "D", Cost = 3 },
                new Path<string> { Source = "D", Destination = "E", Cost = 3 }
                });

            results.Sum(r => r.Cost).Should().Be(9);
            results.Count.Should().Be(3);

            results.First().Cost.Should().Be(3);
            results.First().Source.Should().Be("A");
            results.First().Destination.Should().Be("B");

            results.Skip(1).First().Cost.Should().Be(3);
            results.Skip(1).First().Source.Should().Be("B");
            results.Skip(1).First().Destination.Should().Be("C");

            results.Skip(2).First().Cost.Should().Be(3);
            results.Skip(2).First().Source.Should().Be("C");
            results.Skip(2).First().Destination.Should().Be("D");
        }

        [TestMethod]
        public void Calculate_A_to_D_given_AB_AC_AD_AE_BC_CD_DE__should_be__ACD() {
            var results = Engine.CalculateShortestPathBetween(
                "A",
                "D",
                new[] {
                new Path<string> { Source = "A", Destination = "B", Cost = 3 },
                new Path<string> { Source = "A", Destination = "C", Cost = 3 },
                new Path<string> { Source = "A", Destination = "D", Cost = 7 }, // set this just above ABC (3+3=6)
                new Path<string> { Source = "A", Destination = "E", Cost = 3 },

                new Path<string> { Source = "B", Destination = "C", Cost = 3 },

                new Path<string> { Source = "C", Destination = "D", Cost = 3 },

                new Path<string> { Source = "D", Destination = "E", Cost = 3 }
                });

            results.Sum(r => r.Cost).Should().Be(6);
            results.Count.Should().Be(2);

            results.First().Cost.Should().Be(3);
            results.First().Source.Should().Be("A");
            results.First().Destination.Should().Be("C");

            results.Skip(1).First().Cost.Should().Be(3);
            results.Skip(1).First().Source.Should().Be("C");
            results.Skip(1).First().Destination.Should().Be("D");
        }

        [TestMethod]
        public void Calculate_A_to_D_given_AB_AC_AD_AE_BC_CD_DE__should_be__AD() {
            var results = Engine.CalculateShortestPathBetween(
                "A",
                "D",
                new[] {
                new Path<string> { Source = "A", Destination = "B", Cost = 3 },
                new Path<string> { Source = "A", Destination = "C", Cost = 3 },
                new Path<string> { Source = "A", Destination = "D", Cost = 5 }, // set this just below ABC (3+3=6)
                new Path<string> { Source = "A", Destination = "E", Cost = 3 },

                new Path<string> { Source = "B", Destination = "C", Cost = 3 },

                new Path<string> { Source = "C", Destination = "D", Cost = 3 },

                new Path<string> { Source = "D", Destination = "E", Cost = 3 }
                });

            results.Sum(r => r.Cost).Should().Be(5);
            results.Count.Should().Be(1);

            results.Single().Cost.Should().Be(5);
            results.Single().Source.Should().Be("A");
            results.Single().Destination.Should().Be("D");
        }


    } // TestClass
}
