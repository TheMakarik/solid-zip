namespace SolidZip.UnitTests.Validators;

public class GlobalValidatorTests
{
    [Theory]
    [InlineData("C:\\", true)]
    [InlineData("D:\\", true)]
    [InlineData("F_\\", false)]
    [InlineData("F:A", false)]
    [InlineData(":\\", false)]
    public void IsLogicalDrive_MustValidateDriveAsExpectedResult(string drive, bool excepted)
    {
        //Arrange
        var globalValidator = new GlobalValidator();
        
        //Act
        var result = globalValidator.IsLogicalDrive(drive);
        
        //Assert
        result.Should().Be(excepted);

    }
}