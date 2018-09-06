﻿using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheCapital
{
public class VehicleGraphicSet
  {
    private List<Material> cachedMatsBodyBase = new List<Material>();
    private int cachedMatsBodyBaseHash = -1;
    public Vehicle vehicle;
    public Graphic bodyGraphic;
    public ActorDamageFlasher flasher;


    public VehicleGraphicSet(Vehicle vehicle)
    {
      this.vehicle = vehicle;
      flasher = new ActorDamageFlasher(vehicle);
    }

    public bool AllResolved => bodyGraphic != null;

    public List<Material> MatsBodyBaseAt(Rot4 facing, ConditionDrawMode bodyCondition = ConditionDrawMode.Pristine)
    {
      var num = facing.AsInt + 1000 * (int) bodyCondition;
      
      if (num != cachedMatsBodyBaseHash)
      {
        cachedMatsBodyBase.Clear();
        cachedMatsBodyBaseHash = num;
        switch (bodyCondition)
        {
          case ConditionDrawMode.Pristine:
            cachedMatsBodyBase.Add(bodyGraphic.MatAt(facing, null));
            break;
          case ConditionDrawMode.Weathered:
            cachedMatsBodyBase.Add(bodyGraphic.MatAt(facing));
            break;
          case ConditionDrawMode.Damaged:
            cachedMatsBodyBase.Add(bodyGraphic.MatAt(facing));
            break;
          case ConditionDrawMode.Destroyed:
            cachedMatsBodyBase.Add(bodyGraphic.MatAt(facing));
            break;
          default:
            cachedMatsBodyBase.Add(bodyGraphic.MatAt(facing));
            break;
        }
      }
      return cachedMatsBodyBase;
    }


    public void ClearCache()
    {
      cachedMatsBodyBaseHash = -1;
    }

    public void ResolveAllGraphics()
    {
      ClearCache();
      GraphicDatabase.Get<Graphic_Multi>(vehicle.story.bodyType.bodyGraphicPath, ShaderDatabase.CutoutSkin, Vector2.one, vehicle.story.bodyColor);
    }
  }
}