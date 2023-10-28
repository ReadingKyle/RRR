using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ParticleSystemExample
{
    public class ExplosionParticleSystem : ParticleSystem
    {
        public ExplosionParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 100) { }

        protected override void InitializeConstants()
        {
            textureFilename = "bomb_explosion";

            minNumParticles = 200;
            maxNumParticles = 300;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(200, 800);

            var lifetime = RandomHelper.NextFloat(0.2f, 0.5f);

            var acceleration = -velocity / lifetime;

            var rotation = 0;

            var angularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);

            base.InitializeParticle(ref p, where);

            p.Initialize(where, velocity, acceleration, lifetime: lifetime, rotation: rotation, angularVelocity: angularVelocity);
        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
            particle.Color = Color.Orange;

            particle.Scale = .1f + .25f * normalizedLifetime;
        }

        public void PlaceExplosion(Vector2 where) => AddParticles(where);

    }
}
