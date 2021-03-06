using System;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework.Aco
{
    [ProjectComponent(Lifestyle.Transient)]
    public class ColonyParametersValidator : IColonyParametersValidator
    {
        // todo how is eating these exceptions???
        public void Validate(IColonyParameters colonyParameters)
        {
            if ( colonyParameters.CostMatrix.Length == 0 )
            {
                throw new ArgumentException("Cost Matrix is not set!");
            }

            if ( colonyParameters.CostPerFeature.Length == 0 )
            {
                throw new ArgumentException("CostPerFeature array is not set!");
            }

            if ( colonyParameters.CostPerFeature.Length != colonyParameters.CostMatrix.Length )
            {
                throw new ArgumentException("CostMatrix and CostPerFeature do not match!");
            }

            if ( colonyParameters.FixedStartNode < 0 )
            {
                throw new ArgumentException(
                    "FixedStartNode '{0}' is less than zero!".Inject(colonyParameters.FixedStartNode));
            }

            if ( colonyParameters.FixedStartNode > colonyParameters.CostPerFeature.Length - 1 )
            {
                throw new ArgumentException(
                    "FixedStartNode '{0}' is greater than maximum line index '{1}'!".Inject(
                                                                                            colonyParameters
                                                                                                .FixedStartNode,
                                                                                            colonyParameters
                                                                                                .CostPerFeature
                                                                                                .Length - 1));
            }
        }
    }
}