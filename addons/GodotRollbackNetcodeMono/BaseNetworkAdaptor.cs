using Godot;
using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public interface INetworkAdaptor
    {
        void AttachNetworkAdaptor(SyncManager syncManager);
        void DetachNetworkAdaptor(SyncManager syncManager);
        void StopNetworkAdaptor(SyncManager syncManager);
        void Poll();
        void SendPing(int peerId, GDDictionary msg);
        void SendPingBack(int peerId, GDDictionary msg);
        void SendRemoteStart(int peerId);
        void SendRemoteStop(int peerId);
        void SendInputTick(int peerId, byte[] msg);
        bool IsNetworkHost();
        bool IsNetworkMasterForNode(Node node);
        int GetNetworkUniqueId();
    }

    public partial class BaseNetworkAdaptor : Godot.RefCounted, INetworkAdaptor
    {
        private void attach_network_adaptor(Godot.GodotObject sync_manager) => AttachNetworkAdaptor((SyncManager)sync_manager);

        public virtual void AttachNetworkAdaptor(SyncManager syncManager) { }

        private void detach_network_adaptor(Godot.GodotObject sync_manager) => DetachNetworkAdaptor((SyncManager)sync_manager);

        public virtual void DetachNetworkAdaptor(SyncManager syncManager) { }

        private void stop_network_adaptor(Godot.GodotObject sync_manager) => StopNetworkAdaptor((SyncManager)sync_manager);

        public virtual void StopNetworkAdaptor(SyncManager syncManager) { }

        private void poll() => Poll();

        public virtual void Poll() { }

        private void send_ping(int peer_id, GDDictionary msg) => SendPing(peer_id, msg);

        public virtual void SendPing(int peerId, GDDictionary msg) {}

        private void send_ping_back(int peer_id, GDDictionary msg) => SendPingBack(peer_id, msg);

        public virtual void SendPingBack(int peerId, GDDictionary msg) {}

        private void send_remote_start(int peer_id) => SendRemoteStart(peer_id);

        public virtual void SendRemoteStart(int peerId) {}

        private void send_remote_stop(int peer_id) => SendRemoteStop(peer_id);

        public virtual void SendRemoteStop(int peerId) {}

        private void send_input_tick(int peer_id, byte[] msg) => SendInputTick(peer_id, msg);

        public virtual void SendInputTick(int peerId, byte[] msg) {}

        private bool is_network_host() => IsNetworkHost();

        public virtual bool IsNetworkHost() => false;

        private bool is_network_master_for_node(Node node) => IsNetworkMasterForNode(node);

        public virtual bool IsNetworkMasterForNode(Node node) => false;

        private int get_network_unique_id() => GetNetworkUniqueId();

        public virtual int GetNetworkUniqueId() => 0;
    }
}
