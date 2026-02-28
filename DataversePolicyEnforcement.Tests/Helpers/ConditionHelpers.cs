using DataversePolicyEnforcement.Models;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Extensions;

namespace DataversePolicyEnforcement.Tests.Helpers
{
    public class ConditionHelpers
    {
        readonly dpe_PolicyCondition _metCondition;
        readonly dpe_PolicyCondition _notMetCondition;
        readonly IXrmFakedContext _context;

        public ConditionHelpers(
            IXrmFakedContext context,
            dpe_PolicyCondition metCondition,
            dpe_PolicyCondition notMetCondition
        )
        {
            _context = context;
            _metCondition = metCondition;
            _notMetCondition = notMetCondition;
        }

        public void AddConditionToRule(dpe_PolicyRule rule, bool met = true, int sequence = 1)
        {
            var condition = (met ? _metCondition : _notMetCondition)
                .Clone()
                .ToEntity<dpe_PolicyCondition>();
            condition.dpe_PolicyRuleId = rule.ToEntityReference();
            condition.dpe_Sequence = sequence;
            _context.AddEntity(condition);
        }
    }
}
