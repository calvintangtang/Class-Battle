using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : Singleton<TickManager> {

    public int Tick;
    public float tickPeriod = 0.1f;
    public bool bRunning;
    public Coroutine tickCoroutine;

    public List<ITickRecv> tickRecvList = new List<ITickRecv>();
    public List<byte[]> tickDataStack = new List<byte[]>();
    public List<byte[]> CloneTickStack = new List<byte[]>();

    public List<List<byte[]>> BattleTicks = new List<List<byte[]>>();
    public List<List<byte[]>> ReplayBattleTick;

    public void registerTickRecv(ITickRecv recv)
    {
        tickRecvList.Add(recv);
    }

    public void removePlayer(ITickRecv recv) {
        tickRecvList.Remove(recv);
    }

    public void StartTick()
    {
        tickCoroutine = StartCoroutine(SimulateLocalTick());
    }

    IEnumerator SimulateLocalTick()
    {
        while(true) //in battle
        {
            Tick++;
            SendTickToRecvList();
            yield return new WaitForSeconds(tickPeriod);
        }
    }

    public void SendTickToRecvList()
    {
        foreach (var tickRecv in tickRecvList)
        {
            tickRecv.UpdateTick(CloneTickStack);
        }
        CloneTickStack = tickDataStack.DeepClone<List<byte[]>>();
        BattleTicks.Add(CloneTickStack);
        Clear();
    }

    public void StartReplayTick(List<List<byte[]>> playerTicks) {
        tickCoroutine = StartCoroutine(SimulateReplayTick(playerTicks));
    }

    IEnumerator SimulateReplayTick(List<List<byte[]>> playerTicks)
    {
        while (true) //in battle
        {
            Tick++;
            SendReplayTickToRecvList(playerTicks[Tick - 1]);
            yield return new WaitForSeconds(tickPeriod);
        }
    }

    public void SendReplayTickToRecvList(List<byte[]> playerTick)
    {
        foreach (var tickRecv in tickRecvList)
        {
            tickRecv.UpdateTick(playerTick);
        }
    }

    public void ReceiveTick(byte[] data)
    {
        tickDataStack.Add(data);
    }

    public void endBattle()
    {
        BattleSceneControl.Instance.destroyPlayer();
        if (!BattleSceneControl.Instance.bReplay)
        {
            //SaveLoad.Save();
        }
        Debug.Log(Tick);
        Debug.Log(BattleTicks.Count);
        Tick = 0;
        StopCoroutine(tickCoroutine);
    }

    public void Clear()
    {
        tickDataStack.Clear();
    }
}