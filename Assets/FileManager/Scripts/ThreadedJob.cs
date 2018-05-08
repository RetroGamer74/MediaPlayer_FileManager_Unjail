using System.Collections;

public class ThreadedJob
{
    private bool _bIsDone = false; // Used internally to check when task is complete
    private object _tHandle = new object(); // Used to lock when necessary
    private System.Threading.Thread _tThread = null; // The Thread
    
    public bool IsDone // Public thread safe version of _bIsDone
    {
        get
        {
            bool bTmp;
            lock (_tHandle)
            {
                bTmp = _bIsDone;
            }
            return bTmp;
        }
        set
        {
            lock (_tHandle)
            {
                _bIsDone = value;
            }
        }
    }

    public virtual void Start() // Create thread and start it
    {
        _tThread = new System.Threading.Thread(Run);
        _tThread.Start();
    }

    private void Run() // Call the thread function and make sure IsDone is correctly set when it's complete
    {
        ThreadFunction();
        IsDone = true;
    }

    protected virtual void ThreadFunction() { } // Function to override to implement actual thread job

    public virtual bool Update() // Has to be called on the main thread. It makes sure the closing function is called when thread has finished
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    protected virtual void OnFinished() { } // Function to override to do want you want when thread is complete

    public virtual void Abort() // Cancel the thread
    {
        _tThread.Abort();
    }
    
    public IEnumerator WaitFor() // Coroutine version of Update
    {
        while (!Update())
        {
            yield return null;
        }
    }
}