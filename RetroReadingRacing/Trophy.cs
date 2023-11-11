using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RetroReadingRacing
{
    public class Trophy
    {
        Model _model;
        Matrix view;
        Matrix projection;

        float rotationAngle = 0.0f; // Variable to accumulate rotation over time
        float scaleValue = 1.0f / 20.0f; // Variable to accumulate scaling over time

        public Trophy (Model model)
        {
            _model = model;
            view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 5 / 3, 0.1f, 100.0f);
        }

        public void Update(GameTime gameTime)
        {
            float rotationSpeed = MathHelper.ToRadians(90);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotationAngle += rotationSpeed * deltaTime;

            float scaleSpeed = 0.5f;
            scaleValue += scaleSpeed * deltaTime;
            scaleValue = MathHelper.Clamp(scaleValue, 0.0f, 0.5f);
        }

        public void Draw(GameTime gameTime)
        {
            // Scale the model based on the accumulated scale value
            Matrix scale = Matrix.CreateScale(scaleValue);

            // Rotate the model around the Y-axis based on the accumulated angle
            Matrix rotationY = Matrix.CreateRotationY(MathHelper.ToRadians(-90));
            Matrix rotationX = Matrix.CreateRotationX(rotationAngle);
            Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));

            Matrix translate = Matrix.CreateTranslation(new Vector3(0, 0, -3f));

            // Combine the transformations
            Matrix world = scale * rotationY * rotationX * rotationZ * translate;

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    // Set other effect properties as needed
                }

                mesh.Draw();
            }
        }
    }
}
