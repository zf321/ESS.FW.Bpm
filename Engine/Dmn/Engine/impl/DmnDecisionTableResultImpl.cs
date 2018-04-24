using System;
using System.Collections;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    [Serializable]
    public class DmnDecisionTableResultImpl : IDmnDecisionTableResult
    {


        public static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal readonly IList<IDmnDecisionRuleResult> ruleResults;

        public DmnDecisionTableResultImpl(IList<IDmnDecisionRuleResult> ruleResults)
        {
            this.ruleResults = ruleResults;
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

        public IDmnDecisionRuleResult FirstResult
        {
            get
            {
                if (ruleResults == null || ruleResults.Count == 0)
                    return null;
                else return ruleResults[0];
            }
        }

        public IDmnDecisionRuleResult SingleResult
        {
            get
            {
                if (Count == 1)
                {
                    return ruleResults[0];
                }
                else if (Count == 0 || ruleResults == null)
                {
                    return null;
                }
                else
                {
                    throw LOG.decisionResultHasMoreThanOneOutput(this);
                }
            }
        }

        public int Count
        {
            get { return ruleResults.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IDmnDecisionRuleResult this[int index]
        {
            get { return ruleResults[index]; }

            set { ruleResults[index] = value; }
        }

        public List<T> CollectEntries<T>(string outputName)
        {
            List<T> r = new List<T>();
            foreach (var item in ruleResults)
            {
                if (item.ContainsKey(outputName))
                {
                    r.Add((T)item[outputName]);
                }
            }
            return r;
        }

        public T GetSingleEntry<T>()
        {
            IDmnDecisionRuleResult result = SingleResult;
            if (result != null)
            {
                return result.getSingleEntry<T>();
            }
            else
            {
                return default(T);
            }
        }

        public T GetSingleEntryTyped<T>()
        {
            IDmnDecisionRuleResult result = SingleResult;
            if (result != null)
            {
                return result.getSingleEntryTyped<T>();
            }
            else
            {
                return default(T) ;
            }
        }

        public int IndexOf(IDmnDecisionRuleResult item)
        {
            return ruleResults.IndexOf(item);
        }

        public void Insert(int index, IDmnDecisionRuleResult item)
        {
            ruleResults.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ruleResults.RemoveAt(index);
        }

        public void Add(IDmnDecisionRuleResult item)
        {
            ruleResults.Add(item);
        }

        public void Clear()
        {
            ruleResults.Clear();
        }

        public bool Contains(IDmnDecisionRuleResult item)
        {
            return ruleResults.Contains(item);
        }

        public void CopyTo(IDmnDecisionRuleResult[] array, int arrayIndex)
        {
            ruleResults.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDmnDecisionRuleResult item)
        {
            return ruleResults.Remove(item);
        }

        public IEnumerator<IDmnDecisionRuleResult> GetEnumerator()
        {
            return ruleResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ruleResults.GetEnumerator();
        }


        public static DmnDecisionTableResultImpl Wrap(IDmnDecisionResult decisionResult)
        {
            IList<IDmnDecisionRuleResult> ruleResults = new List<IDmnDecisionRuleResult>();

            foreach (var result in decisionResult)
            {
                var ruleResult = new DmnDecisionRuleResultImpl();
                ruleResult.putAllValues(result.EntryMapTyped);

                ruleResults.Add(ruleResult);
            }

            return new DmnDecisionTableResultImpl(ruleResults);
        }
    }
}