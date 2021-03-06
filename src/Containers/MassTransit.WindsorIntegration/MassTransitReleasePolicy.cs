// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al..
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Releasers;
    using Magnum.Extensions;

    //http://stw.castleproject.org/Windsor.Release-Policy.ashx
    public class MassTransitReleasePolicy
        : LifecycledComponentsReleasePolicy
    {
        public override void Track(object instance, Burden burden)
        {
            if (instance.GetType().FullName.StartsWith("MassTransit") ||
                instance.Implements(typeof(Consumes<>.All)))
            {
                return;
            }

            base.Track(instance, burden);
        }
    }
}