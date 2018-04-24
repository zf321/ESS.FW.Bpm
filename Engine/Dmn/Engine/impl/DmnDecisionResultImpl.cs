using ESS.FW.Bpm.Engine.Variable.Value;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    [Serializable]
    public class DmnDecisionResultImpl : IDmnDecisionResult
    {
        

        public static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal readonly IList<IDmnDecisionResultEntries> ruleResults;

        public DmnDecisionResultImpl(IList<IDmnDecisionResultEntries> ruleResults)
        {
            this.ruleResults = ruleResults;
        }

        public virtual IDmnDecisionResultEntries FirstResult
        {
            get
            {
                if (ruleResults.Count > 0)
                    return ruleResults[0];
                return null;
            }
        }

        public virtual IDmnDecisionResultEntries SingleResult
        {
            get
            {
                if (ruleResults.Count == 1)
                    return Get(0);
                if (ruleResults.Count == 0)
                    return null;
                throw LOG.decisionResultHasMoreThanOneOutput(this);
            }
        }
        
        public virtual IList<T> CollectEntries<T>(string outputName)
        {
            IList<T> outputValues = new List<T>();

            foreach (var ruleResult in ruleResults)
                if (ruleResult.ContainsKey(outputName))
                {
                    var value = ruleResult[outputName];
                    outputValues.Add((T) value);
                }

            return outputValues;
        }

        public virtual IList<IDictionary<string, object>> ResultList
        {
            get
            {
                IList<IDictionary<string, object>> entryMapList = new List<IDictionary<string, object>>();

                foreach (var ruleResult in ruleResults)
                {
                    var entryMap = ruleResult.EntryMap;
                        entryMapList.Add(entryMap);
                }

                return entryMapList;
            }
        }

        public virtual T GetSingleEntryTyped<T>()
        {
            var result = SingleResult;
            if (result != null)
            {

                return (T)result.GetSingleEntryTyped<T>();
            }
            return default(T);
        }
        public int Count
        {
            get { return ruleResults.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IDmnDecisionResultEntries this[int index]
        {
            get { return ruleResults[index]; }

            set { ruleResults[index] = value; }
        }

        public IDmnDecisionResultEntries GetSingleEntry()
        {
            if (ruleResults.Count == 1)
                return Get(0);
            if (ruleResults.Count == 0)
                return null;
            throw LOG.decisionResultHasMoreThanOneOutput(this);
        }

        public int IndexOf(IDmnDecisionResultEntries item)
        {
            return ruleResults.IndexOf(item);
        }

        public void Insert(int index, IDmnDecisionResultEntries item)
        {
            ruleResults.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ruleResults.RemoveAt(index);
        }

        public void Add(IDmnDecisionResultEntries item)
        {
            ruleResults.Add(item);
        }

        public void Clear()
        {
            ruleResults.Clear();
        }

        public bool Contains(IDmnDecisionResultEntries item)
        {
            return ruleResults.Contains(item);
        }

        public void CopyTo(IDmnDecisionResultEntries[] array, int arrayIndex)
        {
            ruleResults.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDmnDecisionResultEntries item)
        {
            return ruleResults.Remove(item);
        }

        public IEnumerator<IDmnDecisionResultEntries> GetEnumerator()
        {
            return ruleResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ruleResults.GetEnumerator();
        }

        public IDmnDecisionResultEntries Get(int index)
        {
            return ruleResults[index];
        }

        public override string ToString()
        {
            return ruleResults.ToString();
        }
    }
}