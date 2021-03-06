﻿using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Npc.Services
{
    public class NpcDomainService : INpcDomainService
    {
        private readonly IRepository<NpcEntity> _npcRepository;
        private readonly IRepository<ScriptEntity> _scriptRepository;
        private readonly IRepository<NpcScriptEntity> _npcScriptRepository;
        private readonly IRepository<ScriptCommandEntity> _scriptCommandERepository;
        private readonly IMemoryCache _cache;

        public NpcDomainService(
            IRepository<NpcEntity> npcRepository, 
            IRepository<ScriptEntity> scriptRepository, 
            IRepository<NpcScriptEntity> npcScriptRepository, 
            IRepository<ScriptCommandEntity> scriptCommandERepository,
            IMemoryCache cache)
        {
            _npcRepository = npcRepository;
            _scriptRepository = scriptRepository;
            _npcScriptRepository = npcScriptRepository;
            _scriptCommandERepository = scriptCommandERepository;
            _cache = cache;
        }

        public async Task<IQueryable<NpcEntity>> GetAll()
        {
            return await _npcRepository.GetAll();
        }

        public async Task<NpcEntity> Get(Expression<Func<NpcEntity, bool>> where)
        {
            return await _npcRepository.Get(where);
        }

        public async Task<NpcEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Npc, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _npcRepository.Get(id);
            });
        }

        public async Task Add(NpcEntity npc)
        {
            await _npcRepository.Add(npc);
        }

        public async Task Update(NpcEntity npc)
        {
             await _npcRepository.Update(npc);
        }

        public async Task Delete(NpcEntity npc)
        {
            await _npcRepository.Remove(npc);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
