using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 所有工作单元类的基类
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        /// <summary>
        /// 标识Begin方法之前是否调用过
        /// </summary>
        private bool _isBeginCalledBefore;
        /// <summary>
        /// 标识Complete方法之前是否调用过
        /// </summary>
        private bool _isCompleteCalledBefore;
        /// <summary>
        /// 标识工作单元是否顺利完成
        /// </summary>
        private bool _succeed;
        /// <summary>
        /// 如果此工作单元失败，则引用该异常
        /// </summary>
        private Exception _exception;
        /// <summary>
        /// 工作单元标识
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// 如果存在，则引用外部UOW（工作单元链 ）
        /// </summary>
        public IUnitOfWork Outer { get; set; }
        /// <summary>
        /// 工作单位提交成功的后处理事件
        /// </summary>
        public event EventHandler Completed;
        /// <summary>
        /// 工作单位提交失败的后处理事件
        /// </summary>
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        /// <summary>
        /// 工作单元注销的后处理事件
        /// </summary>
        public event EventHandler Disposed;
        /// <summary>
        /// 当前工作单元是否已Disposed
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// 当前工作单元的参数
        /// </summary>
        public UnitOfWorkOptions Options { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected UnitOfWorkBase()
        {
            this.Id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 启动工作单元
        /// </summary>
        public void Begin(UnitOfWorkOptions options)
        {
            Check.NotNull(options, nameof(options));

            PreventMultipleBegin();
            this.Options = options;

            BeginUow();
        }

        /// <summary>
        /// 完成这个工作单元。保存所有更改并提交事务（如果存在）
        /// </summary>
        public void Complete()
        {
            PreventMultipleComplete();
            try
            {
                CompleteUow();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 注销工作单元
        /// </summary>
        public void Dispose()
        {
            if (!_isBeginCalledBefore || IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                OnFailed(_exception);
            }

            DisposeUow();
            OnDisposed();
        }

        /// <summary>
        /// 可以通过派生类来实现UOW启动
        /// </summary>
        protected virtual void BeginUow() { }

        /// <summary>
        /// 可以通过派生类来实现UOW提交
        /// </summary>
        protected abstract void CompleteUow();
        /// <summary>
        /// 可以通过派生类来实现UOW注销
        /// </summary>
        protected abstract void DisposeUow();


        protected virtual void OnCompleted()
        {
            if (Completed == null)
                return;
            Completed(this, EventArgs.Empty);
        }

        protected virtual void OnFailed(Exception exception)
        {
            if (Failed == null)
                return;
            Failed(this, new UnitOfWorkFailedEventArgs(exception));
        }

        protected virtual void OnDisposed()
        {
            if (Disposed == null)
                return;
            Disposed(this, EventArgs.Empty);
        }

        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
                throw new MapleException("This unit of work has started before. Can not call Start method more than once.");
            _isBeginCalledBefore = true;
        }

        private void PreventMultipleComplete()
        {
            if (_isCompleteCalledBefore)
                throw new MapleException("Complete is called before!");
            _isCompleteCalledBefore = true;
        }
 
        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}
