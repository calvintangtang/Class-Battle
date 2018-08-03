using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITickRecv {
    void RegisterTickRev();
    void SendTick();
    void UpdateTick(List<byte[]> tickData);
}
