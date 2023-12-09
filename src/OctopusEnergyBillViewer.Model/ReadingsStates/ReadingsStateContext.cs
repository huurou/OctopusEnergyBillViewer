namespace OctopusEnergyBillViewer.Model.ReadingsStates;
public record class ReadingsStateContext
{
    private ReadingsStateBase state_ = new ReadingsInitState();

    public void Start()
    {
        state_ = new ReadingsInitState();
        state_.Entry();
    }

    protected void Transit(ReadingsStateBase state)
    {
        state_.Exit();
        state_ = state;
        state_.Entry();
    }
}

public abstract record class ReadingsStateBase
{
    public virtual void Entry()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void ReadingsFetchSucceeded()
    {
    }

    public virtual void ReadingsFetch1Failed()
    {
    }

    public virtual void ReadingsFetch2Failed()
    {
    }

    public virtual void AccessTokenRefreshSucceeded()
    {
    }

    public virtual void AccessTokenRefreshFailed()
    {
    }

    public virtual void LoginSucceeded()
    {
    }

    public virtual void LoginFailed()
    {
    }
}

public record class ReadingsInitState : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}

public record class ReadingsFetch1State : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}
public record class ReadingsFetch2State : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}

public record class ReadingsAccessTokenObtainState : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}

public record class ReadingsLoginState : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}

public record class ReadingsLoginInfoRequestState : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}

public record class ReadingsAccountNumberRequestState : ReadingsStateBase
{
    public override void Entry()
    {
        base.Entry();
    }
}