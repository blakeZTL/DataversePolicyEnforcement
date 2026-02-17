using DataversePolicyEnforcement.CustomApi;
using DataversePolicyEnforcement.Models;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Plugins.Middleware.CustomApis;

namespace DataversePolicyEnforcement.Tests.CustomApi.FakeMessageExecutors
{
    public class DPE_GetAttributeDecisionFakeMessageExecutor
        : CustomApiFakeMessageExecutor<
            DPE_GetAttributeDecisions,
            dpe_GetAttributeDecisionsRequest,
            dpe_GetAttributeDecisionsResponse
        >,
            ICustomApiFakeMessageExecutor
    {
        //DataversePolicyEnforcement.CustomApi.DPE_GetAttributeDecisions
    }
}
