using ESys.Application.Contracts.Libraries;
using org.matheval;

namespace ESys.Libraries.Helpers;

public class ExpHelper : IExpHelper
{
    public Dictionary<string, string> ApplyExpsOnData(Dictionary<string, string> dataPool,
        Dictionary<string, string> expPool)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        Expression expression = new Expression();

        foreach (var item in dataPool)
        {
            // todo: find a better approach
            try
            {
                expression.Bind(item.Key, Convert.ToDouble(item.Value));
            }
            catch (Exception)
            {
                expression.Bind(item.Key, item.Value);
            }
        }

        foreach (var exp in expPool)
        {
            try
            {
                expression.SetFomular(exp.Value);
                Object value = expression.Eval();
                result.Add(exp.Key, value.ToString());
            }
            catch (Exception e)
            {
                throw new ArithmeticException($"Exp with key:{exp.Key} and val:{exp.Value} could not be evaluated.");
            }
        }

        return result;
    }
}