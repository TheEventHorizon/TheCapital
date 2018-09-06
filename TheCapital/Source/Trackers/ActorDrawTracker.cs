﻿namespace TheCapital.Trackers
{
  public class Pawn_DrawTracker
  {
    private Actor actor;
    public PawnTweener tweener;
    private JitterHandler jitterer;
    public PawnLeaner leaner;
    public PawnRenderer renderer;
    public PawnUIOverlay ui;
    private PawnFootprintMaker footprintMaker;
    private PawnBreathMoteMaker breathMoteMaker;
    private const float MeleeJitterDistance = 0.5f;

    public Pawn_DrawTracker(Pawn pawn)
    {
      this.pawn = pawn;
      this.tweener = new PawnTweener(pawn);
      this.jitterer = new JitterHandler();
      this.leaner = new PawnLeaner(pawn);
      this.renderer = new PawnRenderer(pawn);
      this.ui = new PawnUIOverlay(pawn);
      this.footprintMaker = new PawnFootprintMaker(pawn);
      this.breathMoteMaker = new PawnBreathMoteMaker(pawn);
    }

    public Vector3 DrawPos
    {
      get
      {
        this.tweener.PreDrawPosCalculation();
        Vector3 vector3 = this.tweener.TweenedPos + this.jitterer.CurrentOffset + this.leaner.LeanOffset;
        vector3.y = this.pawn.def.Altitude;
        return vector3;
      }
    }

    public void DrawTrackerTick()
    {
      if (!this.pawn.Spawned || Current.ProgramState == ProgramState.Playing && !Find.CameraDriver.CurrentViewRect.ExpandedBy(3).Contains(this.pawn.Position))
        return;
      this.jitterer.JitterHandlerTick();
      this.footprintMaker.FootprintMakerTick();
      this.breathMoteMaker.BreathMoteMakerTick();
      this.leaner.LeanerTick();
      this.renderer.RendererTick();
    }

    public void DrawAt(Vector3 loc)
    {
      this.renderer.RenderPawnAt(loc);
    }

    public void Notify_Spawned()
    {
      this.tweener.ResetTweenedPosToRoot();
    }

    public void Notify_WarmingCastAlongLine(ShootLine newShootLine, IntVec3 ShootPosition)
    {
      this.leaner.Notify_WarmingCastAlongLine(newShootLine, ShootPosition);
    }

    public void Notify_DamageApplied(DamageInfo dinfo)
    {
      if (this.pawn.Destroyed)
        return;
      this.jitterer.Notify_DamageApplied(dinfo);
      this.renderer.Notify_DamageApplied(dinfo);
    }

    public void Notify_DamageDeflected(DamageInfo dinfo)
    {
      if (this.pawn.Destroyed)
        return;
      this.jitterer.Notify_DamageDeflected(dinfo);
    }

    public void Notify_MeleeAttackOn(Thing Target)
    {
      if (Target.Position != this.pawn.Position)
      {
        this.jitterer.AddOffset(0.5f, (Target.Position - this.pawn.Position).AngleFlat);
      }
      else
      {
        if (!(Target.DrawPos != this.pawn.DrawPos))
          return;
        this.jitterer.AddOffset(0.25f, (Target.DrawPos - this.pawn.DrawPos).AngleFlat());
      }
    }

    public void Notify_DebugAffected()
    {
      for (int index = 0; index < 10; ++index)
        MoteMaker.ThrowAirPuffUp(this.pawn.DrawPos, this.pawn.Map);
      this.jitterer.AddOffset(0.05f, (float) Rand.Range(0, 360));
    }
  }
}