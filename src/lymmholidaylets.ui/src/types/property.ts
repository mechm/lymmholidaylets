export interface PropertyHost {
  name: string;
  jobTitle: string;
  numberOfProperties: number;
  yearsExperience: number;
  profileBio?: string;
  location?: string;
  imagePath?: string;
}

export interface PropertyMap {
  showStreetView: boolean;
  latitude: number;
  longitude: number;
  mapZoom: number;
  streetViewLatitude: number;
  streetViewLongitude: number;
  pitch: number;
  yaw: number;
  zoom: number;
}

export interface PropertyRatingSummary {
  rating: number;
  totalReviews: number;
  accuracy?: number;
  cleanliness?: number;
  communication?: number;
  checkInExperience?: number;
  value?: number;
  location?: number;
  facilities?: number;
  comfort?: number;
}

export interface PropertyReview {
  name: string;
  company?: string;
  position?: string;
  description: string;
  rating: number;
  dateAdded?: string;
  reviewType: string;
  linkToView?: string;
  dateToDisplay: string;
}

export interface PropertyFaq {
  question: string;
  answer: string;
}

export interface PropertyImage {
  imagePath: string;
  altText?: string;
  sequenceOrder: number;
}

export interface PropertyBedroom {
  bedroomNumber: number;
  bedroomName?: string;
  bedType: string;
  bedTypeIcon?: string;
  numberOfBeds: number;
}

export interface PropertyShareLinks {
  facebook?: string;
  twitter?: string;
  whatsApp?: string;
  email?: string;
}

export interface PropertySeo {
  title?: string;
  description?: string;
  canonicalUrl?: string;
  ogImage?: string;
}

export interface PropertyDetail {
  propertyId: number;
  displayAddress?: string;
  description?: string;
  slug?: string;
  minimumNumberOfAdult: number;
  maximumNumberOfGuests: number;
  maximumNumberOfAdult: number;
  maximumNumberOfChildren: number;
  maximumNumberOfInfants: number;
  numberOfBedrooms: number;
  numberOfBathrooms: number;
  numberOfReceptionRooms: number;
  numberOfKitchens: number;
  numberOfCarSpaces: number;
  checkInTime: string;
  checkOutTime: string;
  minimumStayNights: number;
  maximumStayNights?: number;
  datesBooked: string[];
  faqs: PropertyFaq[];
  ratingSummary?: PropertyRatingSummary;
  host?: PropertyHost;
  map?: PropertyMap;
  amenities: string[];
  images: PropertyImage[];
  bedrooms: PropertyBedroom[];
  reviews: PropertyReview[];
  shareLinks: PropertyShareLinks;
  seo: PropertySeo;
  schemaOrg: object;
  lastModified?: string;
  videoHtml?: string;
  disclaimer?: string;
}
