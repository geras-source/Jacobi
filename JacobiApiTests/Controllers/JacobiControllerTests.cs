using JacobiApi.Controllers;
using Moq;
using Xunit;
using Jacobi.Services.Jacobi;
using Jacobi.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace JacobiApi.Tests;

public class JacobiControllerTests
{
    [Fact]
    public async Task Calculate_Returns_Ok_With_Answers()
    {
        // Arrange
        var input = new List<double[]>
        {
            new double[] { 1, 4, 7, 10, 13 },
            new double[] { 4, 7, 10, 13, 16 },
            new double[] { 7, 10, 13, 16, 40 },
            new double[] { 10, 13, 16, 19, 22 },
            new double[] { 13, 16, 19, 22, 5 }
        };

        var mockJacobiCalculator = new Mock<IJacobiCalculator>();
        var mockAnswerStorage = new Mock<IAnswerStorage>();

        var expectedAnswers = new List<Answer>
        {
            new Answer
            {
                Matrix = new List<double[]>
                {
                    new double[]
                    {
                        0.49606359451919424, -0.2562913204457675, -0.2260007270334689, -0.13919376881029036,
                        0.16205974046211774
                    },
                    new double[]
                    {
                        0.008355154455937062, 0.36198380919428597, -0.11400328160509166, -0.06593977806940095,
                        -0.4155103910941999
                    },
                    new double[]
                    {
                        0.4449564773531868, 0.13215846676909193, 0.5377161750178546, 0.029829803521802145,
                        -0.17823464766820196
                    },
                    new double[]
                    {
                        0.19432269052427475, -0.2735490849004153, -0.13343577685784522, 0.017576819151120623,
                        -0.1969390918818485
                    },
                    new double[]
                    {
                        0.1174144635524916, 0.0773541461207618, -0.6246355196065456, -0.008258589833798104,
                        0.1290025368591903
                    },

                },
                Values = new[]
                {
                    13.154081221442311,
                    29.145875889077693,
                    -20.54661878870773,
                    12.174460183811114,
                    11.072201494376607
                }
            }
        };
        mockAnswerStorage.Setup(s => s.GetAll()).ReturnsAsync(expectedAnswers);

        var controller = new JacobiController(mockJacobiCalculator.Object, mockAnswerStorage.Object);

        // Act
        var result = await controller.Calculate(input);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualAnswers = Assert.IsType<List<Answer>>(okResult.Value);
        Assert.Equal(expectedAnswers, actualAnswers);
    }
}