using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ESS.FW.Bpm.Engine.Dmn.Impl.Cmd;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.exception.dmn;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Cmmn;
using ESS.FW.Bpm.Model.Dmn;
using IResource = ESS.FW.Bpm.Engine.Repository.IResource;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    ///     
    ///     
    /// </summary>
    public class RepositoryServiceImpl : ServiceImpl, IRepositoryService
    {
        public virtual Encoding DeploymentCharset { get; set; }


        public virtual IDeploymentBuilder CreateDeployment()
        {
            return new DeploymentBuilderImpl(this);
        }

        public virtual IProcessApplicationDeploymentBuilder CreateDeployment(
            IProcessApplicationReference processApplication)
        {
            return new ProcessApplicationDeploymentBuilderImpl(this, processApplication);
        }
        public IDeploymentWithDefinitions DeployWithResult(DeploymentBuilderImpl deploymentBuilder)
        {
            return CommandExecutor.Execute(new DeployCmd(deploymentBuilder));
        }

        public virtual void DeleteDeployment(string deploymentId)
        {
            CommandExecutor.Execute(new DeleteDeploymentCmd(deploymentId, false, false, false));
        }

        public virtual void DeleteDeploymentCascade(string deploymentId)
        {
            CommandExecutor.Execute(new DeleteDeploymentCmd(deploymentId, true, false, false));
        }

        public virtual void DeleteDeployment(string deploymentId, bool cascade)
        {
            CommandExecutor.Execute(new DeleteDeploymentCmd(deploymentId, cascade, false, false));
        }

        public virtual void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners)
        {
            CommandExecutor.Execute(new DeleteDeploymentCmd(deploymentId, cascade, skipCustomListeners, false));
        }
        public void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
        {
            CommandExecutor.Execute(new DeleteDeploymentCmd(deploymentId, cascade, skipCustomListeners, skipIoMappings));
        }
        public virtual void DeleteProcessDefinition(string processDefinitionId)
        {
            DeleteProcessDefinition(processDefinitionId, false);
        }

        public virtual void DeleteProcessDefinition(string processDefinitionId, bool cascade)
        {
            DeleteProcessDefinition(processDefinitionId, cascade, false);
        }

        public virtual void DeleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners)
        {
            CommandExecutor.Execute(new DeleteProcessDefinitionCmd(processDefinitionId, cascade, skipCustomListeners));
        }

        public virtual IQueryable<IProcessDefinition> CreateProcessDefinitionQuery(Expression<Func<ProcessDefinitionEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<ProcessDefinitionEntity>(expression));
        }

        public virtual IQueryable<ICaseDefinition> CreateCaseDefinitionQuery(Expression<Func<ICaseDefinition, bool>> expression)
        {
            throw new NotImplementedException();
            //return CommandExecutor.Execute(new CreateQueryCmd<CaseDefinitionEntity>(expression));
        }

        public virtual IQueryable<IDecisionDefinition> CreateDecisionDefinitionQuery(Expression<Func<DecisionDefinitionEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<DecisionDefinitionEntity>(expression));
        }

        public virtual IQueryable<IDecisionRequirementsDefinition> CreateDecisionRequirementsDefinitionQuery(Expression<Func<DecisionRequirementsDefinitionEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<DecisionRequirementsDefinitionEntity>(expression));
        }

        public virtual IList<string> GetDeploymentResourceNames(string deploymentId)
        {
            return CommandExecutor.Execute(new GetDeploymentResourceNamesCmd(deploymentId));
        }

        public virtual IList<IResource> GetDeploymentResources(string deploymentId)
        {
            return ListExt.ConvertToListT<IResource>(CommandExecutor.Execute(new GetDeploymentResourcesCmd(deploymentId)));
        }

        public virtual Stream GetResourceAsStream(string deploymentId, string resourceName)
        {
            return CommandExecutor.Execute(new GetDeploymentResourceCmd(deploymentId, resourceName));
        }

        public virtual Stream GetResourceAsStreamById(string deploymentId, string resourceId)
        {
            return CommandExecutor.Execute(new GetDeploymentResourceForIdCmd(deploymentId, resourceId));
        }

        public virtual IQueryable<IDeployment> CreateDeploymentQuery(Expression<Func<DeploymentEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<DeploymentEntity>(expression));
        }

        public virtual IProcessDefinition GetProcessDefinition(string processDefinitionId)
        {
            return
                (IProcessDefinition)
                CommandExecutor.Execute(new GetDeployedProcessDefinitionCmd(processDefinitionId, true));
        }

        public virtual void SuspendProcessDefinitionById(string processDefinitionId)
        {
            UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinitionId).Suspend();
        }

        public virtual void SuspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances,
            DateTime suspensionDate)
        {
            UpdateProcessDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .IncludeProcessInstances(suspendProcessInstances)
                .ExecutionDate(suspensionDate)
                .Suspend();
        }

        public virtual void SuspendProcessDefinitionByKey(string processDefinitionKey)
        {
            UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Suspend();
        }

        public virtual void SuspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances,
            DateTime suspensionDate)
        {
            UpdateProcessDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .IncludeProcessInstances(suspendProcessInstances)
                .ExecutionDate(suspensionDate)
                .Suspend();
        }

        public virtual void ActivateProcessDefinitionById(string processDefinitionId)
        {
            UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinitionId).Activate();
        }

        public virtual void ActivateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances,
            DateTime activationDate)
        {
            UpdateProcessDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinitionId)
                .IncludeProcessInstances(activateProcessInstances)
                .ExecutionDate(activationDate)
                .Activate();
        }

        public virtual void ActivateProcessDefinitionByKey(string processDefinitionKey)
        {
            UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(processDefinitionKey).Activate();
        }

        public virtual void ActivateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances,
            DateTime activationDate)
        {
            UpdateProcessDefinitionSuspensionState()
                .ByProcessDefinitionKey(processDefinitionKey)
                .IncludeProcessInstances(activateProcessInstances)
                .ExecutionDate(activationDate)
                .Activate();
        }

        public virtual IUpdateProcessDefinitionSuspensionStateSelectBuilder UpdateProcessDefinitionSuspensionState()
        {
            return new UpdateProcessDefinitionSuspensionStateBuilderImpl(CommandExecutor);
        }

        public virtual Stream GetProcessModel(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetDeploymentProcessModelCmd(processDefinitionId));
        }

        public virtual Stream GetProcessDiagram(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetDeploymentProcessDiagramCmd(processDefinitionId));
        }

        public virtual Stream GetCaseDiagram(string caseDefinitionId)
        {
            //return commandExecutor.execute(new GetDeploymentCaseDiagramCmd(caseDefinitionId));
            return null;
        }

        public virtual DiagramLayout GetProcessDiagramLayout(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetDeploymentProcessDiagramLayoutCmd(processDefinitionId));
        }

        public virtual IBpmnModelInstance GetBpmnModelInstance(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetDeploymentBpmnModelInstanceCmd(processDefinitionId));
        }

        //public virtual CmmnModelInstance getCmmnModelInstance(string caseDefinitionId)
        //{
        //    try
        //    {
        //        return commandExecutor.execute(new GetDeploymentCmmnModelInstanceCmd(caseDefinitionId));
        //    }
        //    catch (NullValueException e)
        //    {
        //        throw new NotValidException(e.Message, e);
        //    }
        //    catch (CmmnModelInstanceNotFoundException e)
        //    {
        //        throw new NotFoundException(e.Message, e);
        //    }
        //    catch (DeploymentResourceNotFoundException e)
        //    {
        //        throw new NotFoundException(e.Message, e);
        //    }
        //}

        public virtual IDmnModelInstance GetDmnModelInstance(string decisionDefinitionId)
        {
            try
            {
                return CommandExecutor.Execute(new GetDeploymentDmnModelInstanceCmd(decisionDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DmnModelInstanceNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch (DeploymentResourceNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual void AddCandidateStarterUser(string processDefinitionId, string userId)
        {
            CommandExecutor.Execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
        }

        public virtual void AddCandidateStarterGroup(string processDefinitionId, string groupId)
        {
            CommandExecutor.Execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
        }

        public virtual void DeleteCandidateStarterGroup(string processDefinitionId, string groupId)
        {
            CommandExecutor.Execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
        }

        public virtual void DeleteCandidateStarterUser(string processDefinitionId, string userId)
        {
            CommandExecutor.Execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
        }

        public virtual IList<IIdentityLink> GetIdentityLinksForProcessDefinition(string processDefinitionId)
        {
            return CommandExecutor.Execute(new GetIdentityLinksForProcessDefinitionCmd(processDefinitionId));
        }

        public virtual ICaseDefinition GetCaseDefinition(string caseDefinitionId)
        {
            try
            {
                throw new NotImplementedException();
                //return CommandExecutor.Execute(new GetDeploymentCaseDefinitionCmd(caseDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (System.Exception e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual Stream GetCaseModel(string caseDefinitionId)
        {
            try
            {
                throw new NotImplementedException();
                //return CommandExecutor.Execute(new GetDeploymentCaseModelCmd(caseDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            //catch (CaseDefinitionNotFoundException e)
            //{
            //    throw new NotFoundException(e.Message, e);
            //}
            catch (DeploymentResourceNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            return null;
        }

        public virtual IDecisionDefinition GetDecisionDefinition(string decisionDefinitionId)
        {
            try
            {
                return CommandExecutor.Execute(new GetDeploymentDecisionDefinitionCmd(decisionDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DecisionDefinitionNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual IDecisionRequirementsDefinition GetDecisionRequirementsDefinition(
            string decisionRequirementsDefinitionId)
        {
            try
            {
                return
                    CommandExecutor.Execute(
                        new GetDeploymentDecisionRequirementsDefinitionCmd(decisionRequirementsDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DecisionDefinitionNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual Stream GetDecisionModel(string decisionDefinitionId)
        {
            try
            {
                return CommandExecutor.Execute(new GetDeploymentDecisionModelCmd(decisionDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DecisionDefinitionNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch (DeploymentResourceNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual Stream GetDecisionRequirementsModel(string decisionRequirementsDefinitionId)
        {
            try
            {
                return
                    CommandExecutor.Execute(
                        new GetDeploymentDecisionRequirementsModelCmd(decisionRequirementsDefinitionId));
            }
            catch (NullValueException e)
            {
                throw new NotValidException(e.Message, e);
            }
            catch (DecisionDefinitionNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch (DeploymentResourceNotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
        }

        public virtual Stream GetDecisionDiagram(string decisionDefinitionId)
        {
            return CommandExecutor.Execute(new GetDeploymentDecisionDiagramCmd(decisionDefinitionId));
        }

        public virtual Stream GetDecisionRequirementsDiagram(string decisionRequirementsDefinitionId)
        {
            return
                CommandExecutor.Execute(new GetDeploymentDecisionRequirementsDiagramCmd(decisionRequirementsDefinitionId));
        }

        public virtual IDeploymentWithDefinitions DeployAndReturnDefinitions(DeploymentBuilderImpl deploymentBuilder)
        {
            return CommandExecutor.Execute(new DeployCmd(deploymentBuilder));
        }

        public virtual IReadOnlyProcessDefinition GetDeployedProcessDefinition(string processDefinitionId)
        {
            return CommandExecutor.Execute((ICommand<IReadOnlyProcessDefinition>)new GetDeployedProcessDefinitionCmd(processDefinitionId, true));

        }



        public void UpdateProcessDefinitionHistoryTimeToLive(string processDefinitionId, int? historyTimeToLive)
        {
            throw new NotImplementedException();
        }

        public void UpdateDecisionDefinitionHistoryTimeToLive(string decisionDefinitionId, int? historyTimeToLive)
        {
            throw new NotImplementedException();
        }

        public void UpdateCaseDefinitionHistoryTimeToLive(string caseDefinitionId, int? historyTimeToLive)
        {
            throw new NotImplementedException();
        }

        public ICmmnModelInstance GetCmmnModelInstance(string caseDefinitionId)
        {
            throw new NotImplementedException();
        }
    }
}