﻿using AutoMapper;
using Emprise.Admin.Models.Npc;
using Emprise.Admin.Models.NpcScript;
using Emprise.Admin.Models.Room;
using Emprise.Admin.Models.Script;
using Emprise.Admin.Models.Tasks;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Tasks.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emprise.Admin.Mapper
{

    public class ModelProfile : Profile
    {
        public ModelProfile()
        {


            CreateMap<RoomEntity, RoomInput>();

            CreateMap<RoomInput, RoomEntity>()
                .ForMember(x => x.EastName, y => y.Ignore())
                .ForMember(x => x.WestName, y => y.Ignore())
                .ForMember(x => x.SouthName, y => y.Ignore())
                .ForMember(x => x.NorthName, y => y.Ignore());

            CreateMap<TaskEntity, TaskInput>();
            CreateMap<TaskInput, TaskEntity>();

            CreateMap<ScriptEntity, ScriptInput>()
                ;

            CreateMap<ScriptInput, ScriptEntity>()
                ;

            CreateMap<NpcEntity, NpcInput>();
            CreateMap<NpcInput, NpcEntity>();

            CreateMap<NpcEntity, NpcModel>();


            CreateMap<ScriptEntity, ScriptCommandInput>();

            CreateMap<ScriptCommandInput, ScriptEntity>();

        }
    }
}
