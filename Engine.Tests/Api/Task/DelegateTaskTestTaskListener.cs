using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Task
{
    public class DelegateTaskTestTaskListener : ITaskListener
    {
        public const string VARNAME_CANDIDATE_USERS = "candidateUsers";
        public const string VARNAME_CANDIDATE_GROUPS = "candidateGroups";

        public virtual void Notify(IDelegateTask delegateTask)
        {
            var candidates = delegateTask.Candidates;
            ISet<string> candidateUsers = new HashSet<string>();
            ISet<string> candidateGroups = new HashSet<string>();
            foreach (var candidate in candidates)
                if (candidate.UserId != null)
                    candidateUsers.Add(candidate.UserId);
                else if (candidate.GroupId != null)
                    candidateGroups.Add(candidate.GroupId);
            delegateTask.SetVariable(VARNAME_CANDIDATE_USERS, candidateUsers);
            delegateTask.SetVariable(VARNAME_CANDIDATE_GROUPS, candidateGroups);
        }
    }
}