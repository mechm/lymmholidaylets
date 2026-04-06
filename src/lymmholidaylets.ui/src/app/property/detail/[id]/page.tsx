
import Image from 'next/image';
import { notFound } from 'next/navigation';
import type { PropertyDetail } from '@/types/property';

interface PropertyDetailPageProps {
  params: Promise<{ id: string }>;
}

const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:8080';

async function getPropertyDetail(id: string): Promise<PropertyDetail | null> {
  try {
    const res = await fetch(`${API_BASE_URL}/api/v1/property/detail/${id}`, {
      next: { revalidate: 3600 },
    });
    if (res.status === 404) return null;
    if (!res.ok) throw new Error(`Failed to fetch property: ${res.status}`);
    const json = await res.json();
    return json.data ?? json;
  } catch (error) {
    console.error('Error fetching property detail:', error);
    return null;
  }
}

export default async function PropertyDetailPage({ params }: PropertyDetailPageProps) {
  const { id } = await params;
  const property = await getPropertyDetail(id);

  if (!property) return notFound();

  return (
    <main>
      {/* Property header */}
      <section className="property-header">
        <div className="container">
          <h1>{property.displayAddress}</h1>
          {property.ratingSummary && (
            <p>
              ★ {property.ratingSummary.rating.toFixed(2)} &middot;{' '}
              {property.ratingSummary.totalReviews} reviews
            </p>
          )}
        </div>
      </section>

      {/* Images */}
      {property.images.length > 0 && (
        <section className="property-images">
          <div className="container">
            <div className="property-images-grid">
              {property.images.slice(0, 5).map((img) => (
                <div key={img.sequenceOrder} className="property-image-item">
                  <Image
                    src={img.imagePath}
                    alt={img.altText ?? property.displayAddress ?? 'Property image'}
                    width={800}
                    height={600}
                    style={{ width: '100%', height: 'auto', objectFit: 'cover' }}
                  />
                </div>
              ))}
            </div>
          </div>
        </section>
      )}

      <div className="container">
        <div className="property-detail-layout">
          {/* Main content */}
          <div className="property-detail-main">
            {/* Description */}
            {property.description && (
              <section className="property-section">
                <h2>About this property</h2>
                <p>{property.description}</p>
              </section>
            )}

            {/* Room counts */}
            <section className="property-section">
              <h2>Property details</h2>
              <ul className="property-specs">
                <li>{property.numberOfBedrooms} bedroom{property.numberOfBedrooms !== 1 ? 's' : ''}</li>
                <li>{property.numberOfBathrooms} bathroom{property.numberOfBathrooms !== 1 ? 's' : ''}</li>
                {property.numberOfReceptionRooms > 0 && (
                  <li>{property.numberOfReceptionRooms} reception room{property.numberOfReceptionRooms !== 1 ? 's' : ''}</li>
                )}
                {property.numberOfCarSpaces > 0 && (
                  <li>{property.numberOfCarSpaces} car space{property.numberOfCarSpaces !== 1 ? 's' : ''}</li>
                )}
                <li>Sleeps {property.maximumNumberOfGuests}</li>
                <li>Check-in from {property.checkInTime}</li>
                <li>Check-out by {property.checkOutTime}</li>
                <li>Minimum stay: {property.minimumStayNights} night{property.minimumStayNights !== 1 ? 's' : ''}</li>
                {property.maximumStayNights && (
                  <li>Maximum stay: {property.maximumStayNights} nights</li>
                )}
              </ul>
            </section>

            {/* Bedrooms */}
            {property.bedrooms.length > 0 && (
              <section className="property-section">
                <h2>Bedrooms</h2>
                <ul className="bedrooms-list">
                  {property.bedrooms.map((bedroom) => (
                    <li key={bedroom.bedroomNumber}>
                      <strong>{bedroom.bedroomName ?? `Bedroom ${bedroom.bedroomNumber}`}</strong>
                      {' — '}{bedroom.numberOfBeds} × {bedroom.bedType}
                    </li>
                  ))}
                </ul>
              </section>
            )}

            {/* Amenities */}
            {property.amenities.length > 0 && (
              <section className="property-section">
                <h2>Amenities</h2>
                <ul className="amenities-list">
                  {property.amenities.map((amenity) => (
                    <li key={amenity}>{amenity}</li>
                  ))}
                </ul>
              </section>
            )}

            {/* Video */}
            {property.videoHtml && (
              <section className="property-section">
                <h2>Video tour</h2>
                <div dangerouslySetInnerHTML={{ __html: property.videoHtml }} />
              </section>
            )}

            {/* FAQs */}
            {property.faqs.length > 0 && (
              <section className="property-section">
                <h2>Frequently asked questions</h2>
                <dl className="faqs-list">
                  {property.faqs.map((faq, i) => (
                    <div key={i}>
                      <dt><strong>{faq.question}</strong></dt>
                      <dd>{faq.answer}</dd>
                    </div>
                  ))}
                </dl>
              </section>
            )}

            {/* Reviews */}
            {property.reviews.length > 0 && (
              <section className="property-section">
                <h2>
                  Guest reviews
                  {property.ratingSummary && (
                    <span> ({property.ratingSummary.totalReviews} total)</span>
                  )}
                </h2>
                {property.ratingSummary && (
                  <div className="rating-summary">
                    <p>Overall: ★ {property.ratingSummary.rating.toFixed(2)}</p>
                    <ul className="sub-ratings">
                      {property.ratingSummary.cleanliness && <li>Cleanliness: {property.ratingSummary.cleanliness.toFixed(2)}</li>}
                      {property.ratingSummary.accuracy && <li>Accuracy: {property.ratingSummary.accuracy.toFixed(2)}</li>}
                      {property.ratingSummary.communication && <li>Communication: {property.ratingSummary.communication.toFixed(2)}</li>}
                      {property.ratingSummary.checkInExperience && <li>Check-in: {property.ratingSummary.checkInExperience.toFixed(2)}</li>}
                      {property.ratingSummary.value && <li>Value: {property.ratingSummary.value.toFixed(2)}</li>}
                      {property.ratingSummary.location && <li>Location: {property.ratingSummary.location.toFixed(2)}</li>}
                    </ul>
                  </div>
                )}
                <div className="reviews-list">
                  {property.reviews.map((review, i) => (
                    <div key={i} className="review-item">
                      <p className="review-text">&ldquo;{review.description}&rdquo;</p>
                      <p className="review-author">
                        <strong>{review.name}</strong>
                        {review.position && `, ${review.position}`}
                        {review.company && ` at ${review.company}`}
                        {' · '}{review.dateToDisplay}
                      </p>
                    </div>
                  ))}
                </div>
              </section>
            )}

            {/* Disclaimer */}
            {property.disclaimer && (
              <section className="property-section property-disclaimer">
                <p><small>{property.disclaimer}</small></p>
              </section>
            )}
          </div>

          {/* Sidebar */}
          <aside className="property-detail-sidebar">
            {/* Host card */}
            {property.host && (
              <div className="host-card">
                <h2>Your host</h2>
                {property.host.imagePath && (
                  <Image
                    src={property.host.imagePath}
                    alt={property.host.name}
                    width={80}
                    height={80}
                    className="host-avatar"
                    style={{ borderRadius: '50%', objectFit: 'cover' }}
                  />
                )}
                <h3>{property.host.name}</h3>
                <p className="host-job-title">{property.host.jobTitle}</p>
                {property.host.location && (
                  <p className="host-location">📍 {property.host.location}</p>
                )}
                {property.host.profileBio && (
                  <p className="host-bio">{property.host.profileBio}</p>
                )}
                <ul className="host-stats">
                  <li>{property.host.numberOfProperties} propert{property.host.numberOfProperties !== 1 ? 'ies' : 'y'}</li>
                  <li>{property.host.yearsExperience} years experience</li>
                </ul>
              </div>
            )}

            {/* Booking rules summary */}
            <div className="booking-rules-card">
              <h2>Booking info</h2>
              <ul>
                <li>Guests: up to {property.maximumNumberOfGuests}</li>
                <li>Check-in: from {property.checkInTime}</li>
                <li>Check-out: by {property.checkOutTime}</li>
                <li>Min stay: {property.minimumStayNights} night{property.minimumStayNights !== 1 ? 's' : ''}</li>
              </ul>
            </div>
          </aside>
        </div>
      </div>

      {/* Schema.org JSON-LD */}
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(property.schemaOrg) }}
      />
    </main>
  );
}
