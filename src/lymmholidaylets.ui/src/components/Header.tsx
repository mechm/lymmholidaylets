'use client';
import Link from 'next/link';
import Image from 'next/image';
import { useState } from 'react';


export default function Header() {
   const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
   <header>
          <div className="container">
              <div className="header-top">
                  <div className="row">
                      {/* Logo Column */}
                      <div className="col">
                        <Link href="/" title="Lymm Holidays Lets" className="navbar-brand">
                          <Image
                            src="/logo.svg" // Main SVG image path
                            // Note: next/image handles the fallback. We don't use the onerror attribute.
                            width={176} 
                            height={64} 
                            alt="Lymm Holiday Lets"
                            priority={true} // High priority for logo
                            unoptimized={true} // Use unoptimized for SVG logos sometimes
                          />
                        </Link>
                      </div>
                      <div className="col wow fadeInRight visibility: visible; animation-name: fadeInRight;">
                          <ul className="login text-end">
                              <li className="phone">T: <a href="tel:+441925757575">+44 1925 757575</a></li>
                          </ul>
                      </div>                      
                  </div>
              </div>
          </div>

     <nav className="navbar navbar-expand-lg navbar-dark bg-dark" aria-label="Offcanvas navbar large">
      <div className="container">
        
        {/* Mobile Toggler Button */}
        {/* <button
          className="navbar-toggler"
          type="button"
          onClick={toggleMenu}
          aria-controls="offcanvasNavbar2"
          aria-expanded={isMenuOpen}
        >
          <span className="navbar-toggler-icon"></span>
        </button> */}
        
        {/* Offcanvas Menu */}
        {/* <div
          className={`offcanvas offcanvas-end navbar-dark bg-dark ${isMenuOpen ? 'show' : ''}`}
          id="offcanvasNavbar2"
          aria-labelledby="offcanvasNavbar2Label"
          // Inline style added for demonstration, replaces data-bs-dismiss on toggle
          style={{ visibility: isMenuOpen ? 'visible' : 'hidden' }}
        > */}
          <div className="offcanvas-header">
            <h5 className="offcanvas-title" id="offcanvasNavbar2Label">Navigation</h5>
            <button
              type="button"
              className="btn-close btn-close-black"
              aria-label="Close"
              onClick={toggleMenu}
            ></button>
          </div>
          
          <div className="offcanvas-body">
            <ul className="navbar-nav flex-grow-1 pe-3">
              
              {/* About Us Link */}
              <li className="nav-item">
                {/* Replaced asp-route-id="about-us" with standard Next.js path */}
                <Link className="nav-link" href="/pages/about-us">
                  About Us
                </Link>
              </li>
              
              {/* Properties Dropdown (Static Example) */}
              <li className="nav-item dropdown">
                <a 
                  className="nav-link dropdown-toggle" 
                  href="#" 
                  role="button" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  Properties
                </a>
                <ul className="dropdown-menu">
                  {/* Dynamic links now use template literals */}
                  <li><Link className="dropdown-item" href="/properties/1" title="Lymm Village Apartment">Lymm Village Apartment</Link></li>
                  <li><Link className="dropdown-item" href="/properties/2" title="Lymm House">Lymm House</Link></li>
                  <li><Link className="dropdown-item" href="/properties/3" title="Lymm Cottage">Lymm Cottage</Link></li>
                </ul>
              </li>
              
              {/* Reviews Link */}
              <li className="nav-item">
                <Link className="nav-link" href="/reviews">
                  Reviews
                </Link>
              </li>
              
              {/* About Lymm Link */}
              <li className="nav-item">
                <Link className="nav-link" href="/pages/about-lymm">
                  About Lymm
                </Link>
              </li>
              
              {/* Contact Us Link */}
              <li className="nav-item">
                <Link className="nav-link" href="/contact">
                  Contact us
                </Link>
              </li>
            </ul>
          </div>
        </div>
      {/* </div> */}
    </nav>
</header>
  );
}