
namespace SolidZip.UnitTests;

[AttributeUsage(AttributeTargets.Method)]
public class AutoTestDataAttribute()
    : AutoDataAttribute(() =>
    {
        var fixture = new Fixture();
        var supportMutableValueTypesCustomization = new SupportMutableValueTypesCustomization();
        supportMutableValueTypesCustomization.Customize(fixture);
        return fixture.Customize(new AutoFakeItEasyCustomization());
       
    });

