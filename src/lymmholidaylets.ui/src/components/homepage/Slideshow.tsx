 "use client";

import React, { useState, useEffect } from 'react';

// 1. Define the shape of a single Slide
interface Slide {
  imagePath: string;
  imagePathAlt: string;
  captionTitle?: string; // Optional field
  caption?: string;      // Optional field
  link?: string;         // Optional field
}

// 2. Define the Props for the Component
interface HeroSliderProps {
  slides: Slide[];
}

const Slideshow: React.FC<HeroSliderProps> = ({ slides }) => {
  const [current, setCurrent] = useState<number>(0);

  // Auto-play: Switch slide every 5 seconds
  useEffect(() => {
    if (slides.length <= 1) return;

    const timer = setInterval(() => {
      setCurrent((prev) => (prev === slides.length - 1 ? 0 : prev + 1));
    }, 2000);

    return () => clearInterval(timer);
  }, [slides.length]);

  if (!slides || slides.length === 0) return null;

  return (
    <section className="hero">
      <div className="hero-slider" style={{ position: 'relative', overflow: 'hidden' }}>
        {slides.map((slide, index) => {
          const isActive = index === current;

          return (
            <div 
              key={`${slide.imagePath}-${index}`} 
              className={`hero-box ${isActive ? 'active' : 'hidden'}`}
              style={{ 
                display: isActive ? 'block' : 'none',
                transition: 'opacity 0.5s ease-in-out' 
              }}
            >
            <div style={{ display: 'flex', justifyContent: 'center', marginTop: 16, gap: 8 }}>
            {slides.map((_, idx) => (
                <button
                key={idx}
                onClick={() => setCurrent(idx)}
                style={{
                    width: 28,
                    height: 28,
                    borderRadius: '50%',
                    border: idx === current ? '2px solid #333' : '1px solid #bbb',
                    background: idx === current ? '#fff' : '#eee',
                    color: idx === current ? '#333' : '#888',
                    fontWeight: 'bold',
                    cursor: 'pointer',
                    outline: 'none',
                    fontSize: 14,
                    transition: 'all 0.2s'
                }}
                aria-label={`Go to slide ${idx + 1}`}
                >
                {idx + 1}
                </button>
            ))}
            </div>
              <div className="hero-image">
                <img 
                 src={`${slide.imagePath}`} 
                 // src={`/uploads/images/slides/${slide.imagePath}`} 
                  alt={slide.imagePathAlt} 
                  title={slide.imagePathAlt}
                  style={{ width: '100%', height: 'auto' }}
                />
              </div>

              <div className="hero-info">
                <div className="hero-inner-info">
                  {slide.captionTitle && (
                    <h1 className="white-color title-big-text">
                      {slide.captionTitle}
                    </h1>
                  )}
                  
                  {slide.caption && (
                    <p className="white-color title-info">
                      {slide.caption}
                    </p>
                  )}

                  {slide.link && (
                    <a 
                      href={slide.link} 
                      className="button white-bg grey-color" 
                    >
                      Book Now
                    </a>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </section>
  );
};

export default Slideshow;