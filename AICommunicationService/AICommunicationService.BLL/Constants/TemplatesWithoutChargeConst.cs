namespace AICommunicationService.BLL.Constants;
public class TemplatesWithoutChargeConst
{
    public static IEnumerable<string> ListOfFreeTemplates = new List<string>
    {
        RegenerateUseCaseDiagramTemplate,
        RegenerateSequenceDiagramTemplate,
        RegenerateFlowchartTemplate
    };

    public const string RegenerateUseCaseDiagramTemplate = "RegenerateUseCaseDiagramTemplate";
    public const string RegenerateSequenceDiagramTemplate = "RegenerateSequenceDiagramTemplate";
    public const string RegenerateFlowchartTemplate = "RegenerateFlowchartTemplate";
}
