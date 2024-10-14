using ESys.Application.Contracts.Libraries;
using org.matheval;

namespace ESys.Libraries.Helpers;

/// <summary>
/// Includes services related to Exps
/// </summary>
public class ExpHelper : IExpHelper
{
    /// <summary>
    /// Iterates over given Exps and applies them on a given data pool
    /// </summary>
    /// <param name="dataPool">Data pool to apply Exps on</param>
    /// <param name="expPool">Exps to apply on data pool</param>
    /// <returns>A dictionary containing result data</returns>
    /// <exception cref="ArithmeticException">Occurs when an Exp is not applicable on data</exception>
    public Dictionary<string, string> ApplyExpsOnData(Dictionary<string, string> dataPool,
        Dictionary<string, string> expPool)
    {
        Dictionary<string, string> result = new();

        var expression = new Expression();

        foreach (var item in dataPool)
            if (double.TryParse(item.Value, out double d) && !Double.IsNaN(d) && !Double.IsInfinity(d))
                expression.Bind(item.Key, d);
            else
                expression.Bind(item.Key, item.Value);

        foreach (var exp in expPool)
            try
            {
                expression.SetFomular(exp.Value);
                var value = expression.Eval();
                result.Add(exp.Key, value.ToString());
            }
            catch (Exception e)
            {
                throw new ArithmeticException($"Exp with key:{exp.Key} and val:{exp.Value} could not be evaluated.");
            }

        return result;
    }
}