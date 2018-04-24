/*
 * Copyright 2016 camunda services GmbH.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF AuthorizationFields.Any KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class ConditionalModels
    {
        public const string CONDITIONAL_PROCESS_KEY = "processKey";
        public const string SUB_PROCESS_ID = "subProcess";
        public const string BOUNDARY_ID = "boundaryId";
        public const string PROC_DEF_KEY = "Process";
        public const string VARIABLE_NAME = "variable";
        public const string CONDITION_ID = "conditionCatch";
        public const string VAR_CONDITION = "${variable == 1}";
        public const string USER_TASK_ID = "userTask";
    }
}