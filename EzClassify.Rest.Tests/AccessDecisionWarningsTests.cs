using System.Text.Json;
using EzClassify.Contract;
using Xunit;

namespace EzClassify.Tests;

public class AccessDecisionWarningsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AccessWarningsRetainActualFieldsForGrantsAndDenials(bool granted)
    {
        var json = "{\"granted\":" + (granted ? "true" : "false") + """
            ,"rules":[],"warnings":[{"kind":"deprecated","message":"Use the current concept",
            "conceptId":"prior","conceptName":"Prior","categoryId":"controls",
            "replacement":"current","replacementName":"Current","ruleId":"modernize"}]}
            """;
        var decision = ResultParser.ParseAccessDecision(json);
        Assert.Equal(granted, decision.Granted);
        var warning = Assert.Single(decision.Warnings);
        Assert.Equal("prior", warning.ConceptId);
        Assert.Equal("Prior", warning.ConceptName);
        Assert.Equal("controls", warning.CategoryId);
        Assert.Equal("current", warning.Replacement);
        Assert.Equal("Current", warning.ReplacementName);
        Assert.Equal("modernize", warning.RuleId);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(decision.Warnings[0], ResultParser.ParseAccessDecisionElement(document.RootElement).Warnings[0]);
    }

    [Theory]
    [InlineData("{\"granted\":true,\"rules\":[]}")]
    [InlineData("{\"granted\":false,\"rules\":[],\"warnings\":null}")]
    public void MissingOrNullWarningsRefuse(string json)
    {
        Assert.Throws<JsonException>(() => ResultParser.ParseAccessDecision(json));
    }
}
