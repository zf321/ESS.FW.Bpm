using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     <para>
    ///         A walker for walking through an object reference structure (e.g. an execution tree).
    ///         Any visited element can have any number of following elements. The elements are visited
    ///         with a breadth-first approach: The walker maintains a list of next elements to which it adds
    ///         a new elements at the end whenever it has visited an element. The walker stops when it encounters
    ///         an element that fulfills the given <seealso cref="WalkCondition{S}" />.
    ///     </para>
    ///     <para>
    ///         Subclasses define the type of objects and provide the walking behavior.
    ///         
    ///     </para>
    /// </summary>
    public abstract class ReferenceWalker<T>
    {
        protected internal IList<T> CurrentElements;

        protected internal IList<ITreeVisitor<T>> PostVisitor = new List<ITreeVisitor<T>>();

        protected internal IList<ITreeVisitor<T>> PreVisitor = new List<ITreeVisitor<T>>();

        public ReferenceWalker(T initialElement)
        {
            CurrentElements = new List<T>();
            CurrentElements.Add(initialElement);
        }

        public ReferenceWalker(IList<T> initialElements)
        {
            CurrentElements = new List<T>(initialElements);
        }

        public virtual T CurrentElement
        {
            get { return CurrentElements.Count == 0 ? default(T) : CurrentElements[0]; }
        }

        protected internal abstract ICollection<T> NextElements();

        public virtual ReferenceWalker<T> AddPreVisitor(ITreeVisitor<T> collector)
        {
            PreVisitor.Add(collector);
            return this;
        }

        public virtual ReferenceWalker<T> AddPostVisitor(ITreeVisitor<T> collector)
        {
            PostVisitor.Add(collector);
            return this;
        }

        public virtual T WalkWhile()
        {
            return WalkWhile(NullCondition<T>().Compile());
        }

        public virtual T WalkUntil()
        {
            return WalkUntil(NullCondition<T>().Compile());
        }

        public virtual T WalkWhile(Func<T, bool> condition)
        {
            while (!condition.Invoke(CurrentElement))
            {
                foreach (var collector in PreVisitor)
                    collector.Visit(CurrentElement);

                ((List<T>) CurrentElements).AddRange(NextElements());
                CurrentElements.RemoveAt(0);

                foreach (var collector in PostVisitor)
                    collector.Visit(CurrentElement);
                Thread.Sleep(5);
            }
            return CurrentElement;
        }

        public virtual T WalkUntil(Func<T,bool>  condition)
        {
            do
            {
                foreach (var collector in PreVisitor)
                    collector.Visit(CurrentElement);

                ((List<T>) CurrentElements).AddRange(NextElements());
                CurrentElements.RemoveAt(0);

                foreach (var collector in PostVisitor)
                    collector.Visit(CurrentElement);
                Thread.Sleep(5);
            } while (!condition.Invoke(CurrentElement));
            return CurrentElement;
        }
        public Expression<Func<TS, bool>> NullCondition<TS>()
        {
            return element => element == null;
        }

    }
    //public  class WalkCondition< TS>
    //{
    //    public virtual bool IsFulfilled(TS element)
    //    {
    //        return false;
    //    }
    //}

    //public class NullCondition<TS> 
    //{
    //    public  bool IsFulfilled(TS element)
    //    {
    //        return element == null;
    //    }
        
    //}
}