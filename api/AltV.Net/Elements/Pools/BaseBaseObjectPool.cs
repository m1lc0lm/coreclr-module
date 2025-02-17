using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared;
using AltV.Net.Shared.Elements.Entities;

namespace AltV.Net.Elements.Pools
{
    public class BaseBaseObjectPool : IBaseBaseObjectPool
    {
        private readonly IEntityPool<IPlayer> playerPool;

        private readonly IEntityPool<IVehicle> vehiclePool;

        private readonly IBaseObjectPool<IBlip> blipPool;

        private readonly IBaseObjectPool<ICheckpoint> checkpointPool;

        private readonly IBaseObjectPool<IVoiceChannel> voiceChannelPool;

        private readonly IBaseObjectPool<IColShape> colShapePool;

        public BaseBaseObjectPool(IEntityPool<IPlayer> playerPool, IEntityPool<IVehicle> vehiclePool,
            IBaseObjectPool<IBlip> blipPool, IBaseObjectPool<ICheckpoint> checkpointPool,
            IBaseObjectPool<IVoiceChannel> voiceChannelPool, IBaseObjectPool<IColShape> colShapePool)
        {
            this.playerPool = playerPool;
            this.vehiclePool = vehiclePool;
            this.blipPool = blipPool;
            this.checkpointPool = checkpointPool;
            this.voiceChannelPool = voiceChannelPool;
            this.colShapePool = colShapePool;
        }

        public IBaseObject Get(IntPtr entityPointer, BaseObjectType baseObjectType)
        {
            return baseObjectType switch
            {
                BaseObjectType.Player => playerPool.Get(entityPointer),
                BaseObjectType.Vehicle => vehiclePool.Get(entityPointer),
                BaseObjectType.Blip => blipPool.Get(entityPointer),
                BaseObjectType.Checkpoint => checkpointPool.Get(entityPointer),
                BaseObjectType.VoiceChannel => voiceChannelPool.Get(entityPointer),
                BaseObjectType.ColShape => colShapePool.Get(entityPointer),
                _ => default
            };
        }

        ISharedBaseObject IReadOnlyBaseBaseObjectPool.Get(IntPtr entityPointer, BaseObjectType baseObjectType) => Get(entityPointer, baseObjectType);

        public IBaseObject GetOrCreate(ICore core, IntPtr entityPointer, BaseObjectType baseObjectType)
        {
            return baseObjectType switch
            {
                BaseObjectType.Player => playerPool.GetOrCreate(core, entityPointer),
                BaseObjectType.Vehicle => vehiclePool.GetOrCreate(core, entityPointer),
                BaseObjectType.Blip => blipPool.GetOrCreate(core, entityPointer),
                BaseObjectType.Checkpoint => checkpointPool.GetOrCreate(core, entityPointer),
                BaseObjectType.VoiceChannel => voiceChannelPool.GetOrCreate(core, entityPointer),
                BaseObjectType.ColShape => colShapePool.GetOrCreate(core, entityPointer),
                _ => default
            };
        }

        public IBaseObject GetOrCreate(ICore core, IntPtr entityPointer, BaseObjectType baseObjectType, ushort entityId)
        {
            return baseObjectType switch
            {
                BaseObjectType.Player => playerPool.GetOrCreate(core, entityPointer, entityId),
                BaseObjectType.Vehicle => vehiclePool.GetOrCreate(core, entityPointer, entityId),
                BaseObjectType.Blip => blipPool.GetOrCreate(core, entityPointer),
                BaseObjectType.Checkpoint => checkpointPool.GetOrCreate(core, entityPointer),
                BaseObjectType.VoiceChannel => voiceChannelPool.GetOrCreate(core, entityPointer),
                BaseObjectType.ColShape => colShapePool.GetOrCreate(core, entityPointer),
                _ => default
            };
        }

        public bool Remove(IBaseObject entity)
        {
            return Remove(entity.NativePointer, entity.Type);
        }

        public bool Remove(IntPtr entityPointer, BaseObjectType baseObjectType)
        {
            return baseObjectType switch
            {
                BaseObjectType.Player => playerPool.Remove(entityPointer),
                BaseObjectType.Vehicle => vehiclePool.Remove(entityPointer),
                BaseObjectType.Blip => blipPool.Remove(entityPointer),
                BaseObjectType.Checkpoint => checkpointPool.Remove(entityPointer),
                BaseObjectType.VoiceChannel => voiceChannelPool.Remove(entityPointer),
                BaseObjectType.ColShape => colShapePool.Remove(entityPointer),
                _ => false
            };
        }
    }
}