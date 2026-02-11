import Image from 'next/image';
import Link from 'next/link';

const WhyChooseUs = () => {
  const choices = [
    {
      title: "Great connections",
      image: "/images/homepage/resized/1280/1280/great-connections.jpg",
      alt: "Great Connections",
      href: null,
    },
    {
      title: "Lots to offer in the village",
      image: "/images/homepage/resized/1280/1280/lymm-dam.jpg",
      alt: "About Lymm",
      href: "/page/about-lymm",
    },
    {
      title: "Years of experience",
      image: "/images/homepage/resized/1280/1280/kath-laura.jpg",
      alt: "Kath and Laura",
      href: "/page/about-us",
    },
  ];

  return (
    <section className="why-choose-us primary-bg empty-space">
      <div className="container">
        <div className="row">
          <div className="col-md-12">
            <div className="title-box">
              <h2 className="title-text white-color">Why Choose Us?</h2>
            </div>
          </div>
        </div>
        <div className="row">
          {choices.map((item, index) => (
            <div key={index} className="col-md-4">
              <div className="why-choose-us-box">
                <div className="why-choose-us-image">
                  {item.href ? (
                    <Link href={item.href}>
                      <ImageContent item={item} />
                    </Link>
                  ) : (
                    <ImageContent item={item} />
                  )}
                </div>
                <div className="why-choose-us-info">
                  <h4 className="white-color title-small-text">{item.title}</h4>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

// Helper component for the Image to keep code clean
const ImageContent = ({ item }: { item: any }) => (
  <div style={{ position: 'relative', width: '100%', aspectRatio: '1/1' }}>
    <Image
      src={item.image}
      alt={item.alt}
      fill
      sizes="(min-width: 1024px) 33vw, 100vw"
      style={{ objectFit: 'cover' }}
      placeholder="blur"
      blurDataURL="/images/missing-468-351.jpg" // Replaces your onError logic
    />
  </div>
);

export default WhyChooseUs;