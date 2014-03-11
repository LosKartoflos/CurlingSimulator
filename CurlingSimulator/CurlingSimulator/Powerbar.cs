using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CurlingSimulator
{
    class CPowerBar
    {
        Vector2 m_pos;
        Vector2 m_sliderPos;

        Vector2 m_increasePower;

        Texture2D m_texture;
        Texture2D m_slider;

        float m_max, m_min;

        bool goUp;

        public CPowerBar(Texture2D texture, Vector2 pos)
        {
            m_texture = texture;
            m_pos = pos;
            float test = m_texture.Height;
            m_sliderPos = new Vector2(m_pos.X + 2, m_pos.Y + 142);
            m_increasePower = new Vector2(0, -1);
            goUp = true;
            m_max = m_pos.Y - 15;
            m_min = m_pos.Y + 142;
        }

        public void setSlider(Texture2D slider)
        {
            m_slider = slider;
        }

        public float getValue()
        {
            return (m_min - m_sliderPos.Y) / 157;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, m_pos, null, Color.White, 0, m_pos, 0.05f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(m_slider, m_sliderPos, Color.White);

        }

        public void update()
        {
            if (goUp)
                m_sliderPos += m_increasePower;
            else
                m_sliderPos -= m_increasePower;

            if (m_sliderPos.Y <= m_max)
                goUp = false;

            if (m_sliderPos.Y >= m_min)
                goUp = true;
        }
    }
}