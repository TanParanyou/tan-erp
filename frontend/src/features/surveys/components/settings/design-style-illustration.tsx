/* eslint-disable no-restricted-syntax -- token-exempt: SVG artwork palette representing architectural design styles */
"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface DesignStyleIllustrationProps {
  code: string;
  className?: string;
  alt?: string;
}

export function DesignStyleIllustration({
  code,
  className,
  alt,
}: DesignStyleIllustrationProps) {
  const normalizedCode = code.trim().toLowerCase();

  switch (normalizedCode) {
    case "modern_luxury":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Modern Luxury"}
        >
          <defs>
            <linearGradient id="ml-bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0%" stopColor="#0B1B2B" />
              <stop offset="100%" stopColor="#050C14" />
            </linearGradient>
            <linearGradient id="ml-gold" x1="0" y1="0" x2="1" y2="0">
              <stop offset="0%" stopColor="#C5A059" />
              <stop offset="50%" stopColor="#F9E2AF" />
              <stop offset="100%" stopColor="#B38938" />
            </linearGradient>
            <linearGradient id="ml-glow" x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stopColor="#F9E2AF" stopOpacity="0.4" />
              <stop offset="100%" stopColor="#0B1B2B" stopOpacity="0" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#ml-bg)" />
          {/* Marble vein lines */}
          <path d="M 0 35 Q 40 45 80 20 T 160 30" stroke="#1E2F45" strokeWidth="1" fill="none" opacity="0.6" />
          <path d="M 10 90 Q 60 70 110 85 T 160 65" stroke="#1E2F45" strokeWidth="1" fill="none" opacity="0.6" />
          {/* Wall panel moldings */}
          <rect x="12" y="14" width="62" height="72" fill="none" stroke="#16273B" strokeWidth="1.5" />
          <rect x="16" y="18" width="54" height="64" fill="none" stroke="url(#ml-gold)" strokeWidth="0.75" opacity="0.8" />
          <rect x="86" y="14" width="62" height="72" fill="none" stroke="#16273B" strokeWidth="1.5" />
          <rect x="90" y="18" width="54" height="64" fill="none" stroke="url(#ml-gold)" strokeWidth="0.75" opacity="0.8" />
          {/* Warm concealed lighting beam */}
          <polygon points="65,0 95,0 120,70 40,70" fill="url(#ml-glow)" />
          {/* Modern luxury chandelier */}
          <line x1="80" y1="0" x2="80" y2="28" stroke="url(#ml-gold)" strokeWidth="1.5" />
          <circle cx="80" cy="28" r="3" fill="url(#ml-gold)" />
          <ellipse cx="80" cy="38" rx="24" ry="7" fill="none" stroke="url(#ml-gold)" strokeWidth="1.5" />
          <ellipse cx="80" cy="45" rx="14" ry="4" fill="none" stroke="url(#ml-gold)" strokeWidth="1" />
          <circle cx="80" cy="38" r="2.5" fill="#FFF" />
          <circle cx="68" cy="38" r="2" fill="#F9E2AF" />
          <circle cx="92" cy="38" r="2" fill="#F9E2AF" />
          {/* Sleek marble console table */}
          <rect x="25" y="86" width="110" height="4" fill="#D9D9D9" />
          <line x1="25" y1="90" x2="135" y2="90" stroke="url(#ml-gold)" strokeWidth="1" />
          <line x1="40" y1="91" x2="40" y2="110" stroke="url(#ml-gold)" strokeWidth="1.5" />
          <line x1="120" y1="91" x2="120" y2="110" stroke="url(#ml-gold)" strokeWidth="1.5" />
          <text x="80" y="104" textAnchor="middle" fill="#C5A059" fontSize="6.5" fontFamily="monospace" letterSpacing="1">MODERN LUXURY</text>
        </svg>
      );

    case "japandi":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Japandi"}
        >
          <defs>
            <linearGradient id="jp-bg" x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stopColor="#F9F6F0" />
              <stop offset="100%" stopColor="#EFE9DE" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#jp-bg)" />
          {/* Subtle zen sun circle */}
          <circle cx="112" cy="46" r="28" fill="#E8DEC8" opacity="0.6" />
          {/* Vertical light oak wood slats */}
          {[14, 20, 26, 32, 38, 44, 50, 56].map((x) => (
            <rect key={x} x={x} y="16" width="3.5" height="74" fill="#CBB28D" />
          ))}
          {/* Minimalist wabi-sabi ceramic vase */}
          <path
            d="M 108 86 C 105 76 98 72 98 62 C 98 57 103 54 105 52 L 109 52 C 111 54 116 57 116 62 C 116 72 109 76 106 86 Z"
            fill="#8C8275"
          />
          {/* Natural branch / ikebana */}
          <path d="M 107 52 Q 105 35 96 24" stroke="#4A3F35" strokeWidth="1.2" fill="none" strokeLinecap="round" />
          <path d="M 102 38 Q 94 36 90 32" stroke="#4A3F35" strokeWidth="1" fill="none" strokeLinecap="round" />
          <circle cx="89" cy="31" r="1.5" fill="#7D6E5D" />
          <circle cx="95" cy="23" r="1.5" fill="#7D6E5D" />
          {/* Low Japanese oak bench */}
          <rect x="74" y="86" width="72" height="4" fill="#B89B72" />
          <rect x="80" y="90" width="4" height="16" fill="#A88B62" />
          <rect x="138" y="90" width="4" height="16" fill="#A88B62" />
          <text x="80" y="104" textAnchor="middle" fill="#7D6E5D" fontSize="6.5" fontFamily="monospace" letterSpacing="1">JAPANDI / MUJI</text>
        </svg>
      );

    case "modern_contemporary":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Modern Contemporary"}
        >
          <defs>
            <linearGradient id="mc-bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0%" stopColor="#1E293B" />
              <stop offset="100%" stopColor="#0F172A" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#mc-bg)" />
          {/* Architectural linear window grid */}
          <rect x="16" y="14" width="70" height="54" fill="#0B1321" stroke="#334155" strokeWidth="1.5" />
          <line x1="51" y1="14" x2="51" y2="68" stroke="#334155" strokeWidth="1" />
          <line x1="16" y1="41" x2="86" y2="41" stroke="#334155" strokeWidth="1" />
          {/* Geometric floating shelves */}
          <rect x="98" y="22" width="48" height="3" fill="#64748B" />
          <rect x="108" y="38" width="38" height="3" fill="#64748B" />
          <rect x="98" y="54" width="48" height="3" fill="#64748B" />
          {/* Accent decoration */}
          <rect x="104" y="14" width="6" height="8" fill="#38BDF8" opacity="0.8" />
          <circle cx="126" cy="33" r="4" fill="#E2E8F0" />
          {/* Contemporary balanced credenza */}
          <rect x="16" y="74" width="128" height="14" fill="#293548" stroke="#475569" strokeWidth="1" />
          <line x1="58" y1="74" x2="58" y2="88" stroke="#1E293B" strokeWidth="1" />
          <line x1="102" y1="74" x2="102" y2="88" stroke="#1E293B" strokeWidth="1" />
          <line x1="26" y1="88" x2="26" y2="96" stroke="#475569" strokeWidth="1.5" />
          <line x1="134" y1="88" x2="134" y2="96" stroke="#475569" strokeWidth="1.5" />
          <text x="80" y="104" textAnchor="middle" fill="#94A3B8" fontSize="6.5" fontFamily="monospace" letterSpacing="1">CONTEMPORARY</text>
        </svg>
      );

    case "scandinavian":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Scandinavian"}
        >
          <defs>
            <linearGradient id="sc-bg" x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stopColor="#F8FAFC" />
              <stop offset="100%" stopColor="#E2E8F0" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#sc-bg)" />
          {/* A-frame roofline / Nordic house accent */}
          <polygon points="80,14 135,56 125,56 80,22 35,56 25,56" fill="#94A3B8" opacity="0.4" />
          {/* Cozy Nordic cone pendant lamp */}
          <line x1="80" y1="0" x2="80" y2="34" stroke="#475569" strokeWidth="1" />
          <polygon points="80,34 68,46 92,46" fill="#334155" />
          <ellipse cx="80" cy="46" rx="12" ry="2" fill="#E2E8F0" />
          {/* Gentle warm bulb light */}
          <circle cx="80" cy="50" r="12" fill="#FDE047" opacity="0.25" />
          {/* Iconic Nordic round side table */}
          <ellipse cx="80" cy="74" rx="36" ry="6" fill="#F1F5F9" stroke="#CBD5E1" strokeWidth="1" />
          {/* Splayed light birch legs */}
          <line x1="56" y1="78" x2="48" y2="96" stroke="#D4A373" strokeWidth="2" strokeLinecap="round" />
          <line x1="80" y1="80" x2="80" y2="96" stroke="#C29263" strokeWidth="2" strokeLinecap="round" />
          <line x1="104" y1="78" x2="112" y2="96" stroke="#D4A373" strokeWidth="2" strokeLinecap="round" />
          {/* Subtle mug / cozy touch */}
          <rect x="76" y="66" width="6" height="7" rx="1" fill="#64748B" />
          <text x="80" y="104" textAnchor="middle" fill="#64748B" fontSize="6.5" fontFamily="monospace" letterSpacing="1">SCANDINAVIAN</text>
        </svg>
      );

    case "minimalist":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Minimalist"}
        >
          <rect width="160" height="110" fill="#F3F4F6" />
          {/* Monolithic handleless cabinetry block with crisp shadow gaps */}
          <rect x="24" y="20" width="112" height="66" fill="#FFFFFF" stroke="#D1D5DB" strokeWidth="1" />
          {/* Vertical shadow gaps */}
          <line x1="52" y1="20" x2="52" y2="86" stroke="#9CA3AF" strokeWidth="1" />
          <line x1="80" y1="20" x2="80" y2="86" stroke="#9CA3AF" strokeWidth="1" />
          <line x1="108" y1="20" x2="108" y2="86" stroke="#9CA3AF" strokeWidth="1" />
          {/* Horizontal shadow gap */}
          <line x1="24" y1="53" x2="136" y2="53" stroke="#9CA3AF" strokeWidth="1" />
          {/* Recessed bottom plinth / shadow reveal */}
          <rect x="28" y="86" width="104" height="4" fill="#374151" />
          {/* Singular minimalist sculpture */}
          <circle cx="80" cy="14" r="3" fill="#111827" />
          <text x="80" y="103" textAnchor="middle" fill="#4B5563" fontSize="6.5" fontFamily="monospace" letterSpacing="1">MINIMALIST</text>
        </svg>
      );

    case "industrial_loft":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Industrial Loft"}
        >
          <defs>
            <linearGradient id="ind-bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0%" stopColor="#242120" />
              <stop offset="100%" stopColor="#171514" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#ind-bg)" />
          {/* Exposed brickwork patterns */}
          {[18, 30, 42, 54, 66].map((y) => (
            <line key={`b-h-${y}`} x1="14" y1={y} x2="146" y2={y} stroke="#3D322B" strokeWidth="1" />
          ))}
          {/* Black structural steel I-Beam */}
          <rect x="14" y="12" width="132" height="6" fill="#1E293B" stroke="#0F172A" strokeWidth="1" />
          <line x1="14" y1="15" x2="146" y2="15" stroke="#475569" strokeWidth="0.8" />
          {/* Steel column */}
          <rect x="24" y="18" width="6" height="74" fill="#1E293B" stroke="#0F172A" strokeWidth="0.8" />
          {/* Industrial suspended Edison bulb */}
          <line x1="80" y1="18" x2="80" y2="44" stroke="#0F172A" strokeWidth="1.2" />
          <rect x="77" y="44" width="6" height="4" fill="#B45309" />
          <ellipse cx="80" cy="53" rx="6" ry="8" fill="#F59E0B" opacity="0.3" />
          <circle cx="80" cy="52" r="3" fill="#FDE68A" />
          {/* Reclaimed wood & iron workbench */}
          <rect x="42" y="76" width="96" height="6" fill="#78350F" stroke="#451A03" strokeWidth="1" />
          <rect x="46" y="82" width="4" height="16" fill="#1E293B" />
          <rect x="130" y="82" width="4" height="16" fill="#1E293B" />
          <line x1="46" y1="88" x2="134" y2="88" stroke="#1E293B" strokeWidth="1" />
          <text x="80" y="104" textAnchor="middle" fill="#D97706" fontSize="6.5" fontFamily="monospace" letterSpacing="1">INDUSTRIAL LOFT</text>
        </svg>
      );

    case "classic_neo_classic":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Classic Neo Classic"}
        >
          <defs>
            <linearGradient id="cl-bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0%" stopColor="#172233" />
              <stop offset="100%" stopColor="#0B1320" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#cl-bg)" />
          {/* Classical crown molding cornice */}
          <rect x="10" y="10" width="140" height="3" fill="#E2E8F0" />
          <rect x="14" y="13" width="132" height="2" fill="#94A3B8" />
          {/* Neoclassical wall wainscot panels */}
          <rect x="18" y="20" width="34" height="68" fill="none" stroke="#E2E8F0" strokeWidth="1.2" />
          <rect x="21" y="23" width="28" height="62" fill="none" stroke="#64748B" strokeWidth="0.8" />
          <rect x="63" y="20" width="34" height="68" fill="none" stroke="#E2E8F0" strokeWidth="1.2" />
          <rect x="66" y="23" width="28" height="62" fill="none" stroke="#64748B" strokeWidth="0.8" />
          <rect x="108" y="20" width="34" height="68" fill="none" stroke="#E2E8F0" strokeWidth="1.2" />
          <rect x="111" y="23" width="28" height="62" fill="none" stroke="#64748B" strokeWidth="0.8" />
          {/* Fluted column capitals accent */}
          <line x1="18" y1="90" x2="142" y2="90" stroke="#E2E8F0" strokeWidth="1.5" />
          <text x="80" y="104" textAnchor="middle" fill="#CBD5E1" fontSize="6.5" fontFamily="monospace" letterSpacing="1">NEO CLASSIC</text>
        </svg>
      );

    case "vintage_retro":
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Vintage Retro"}
        >
          <defs>
            <linearGradient id="vr-bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0%" stopColor="#303A36" />
              <stop offset="100%" stopColor="#1F2824" />
            </linearGradient>
          </defs>
          <rect width="160" height="110" fill="url(#vr-bg)" />
          {/* Mid-century geometric arch & sunburst motif */}
          <path d="M 40 50 A 24 24 0 0 1 88 50 Z" fill="#D97706" opacity="0.65" />
          <circle cx="118" cy="34" r="14" fill="#0D9488" opacity="0.5" />
          {/* Retro curved credenza with tapered brass legs */}
          <rect x="30" y="58" width="100" height="24" rx="3" fill="#B45309" stroke="#78350F" strokeWidth="1" />
          <line x1="63" y1="58" x2="63" y2="82" stroke="#78350F" strokeWidth="1" />
          <line x1="96" y1="58" x2="96" y2="82" stroke="#78350F" strokeWidth="1" />
          <circle cx="58" cy="70" r="1.5" fill="#FBBF24" />
          <circle cx="68" cy="70" r="1.5" fill="#FBBF24" />
          {/* Splayed legs */}
          <line x1="42" y1="82" x2="36" y2="96" stroke="#92400E" strokeWidth="2" strokeLinecap="round" />
          <line x1="118" y1="82" x2="124" y2="96" stroke="#92400E" strokeWidth="2" strokeLinecap="round" />
          <circle cx="36" cy="96" r="1" fill="#FBBF24" />
          <circle cx="124" cy="96" r="1" fill="#FBBF24" />
          <text x="80" y="104" textAnchor="middle" fill="#F59E0B" fontSize="6.5" fontFamily="monospace" letterSpacing="1">VINTAGE RETRO</text>
        </svg>
      );

    default:
      return (
        <svg
          viewBox="0 0 160 110"
          className={cn("w-full h-full", className)}
          role="img"
          aria-label={alt ?? "Design Style"}
        >
          <rect width="160" height="110" fill="#F8FAFC" />
          {/* Architectural drafting grid */}
          {[20, 40, 60, 80, 100, 120, 140].map((x) => (
            <line key={`g-x-${x}`} x1={x} y1="0" x2={x} y2="110" stroke="#E2E8F0" strokeWidth="0.75" />
          ))}
          {[20, 40, 60, 80, 100].map((y) => (
            <line key={`g-y-${y}`} x1="0" y1={y} x2="160" y2={y} stroke="#E2E8F0" strokeWidth="0.75" />
          ))}
          {/* Blueprint drafting compass / icon motif */}
          <circle cx="80" cy="50" r="22" fill="#0B3056" opacity="0.08" />
          <circle cx="80" cy="50" r="16" fill="none" stroke="#0B3056" strokeWidth="1.2" strokeDasharray="3 3" />
          <line x1="80" y1="24" x2="80" y2="76" stroke="#0B3056" strokeWidth="1" />
          <line x1="54" y1="50" x2="106" y2="50" stroke="#0B3056" strokeWidth="1" />
          <rect x="75" y="45" width="10" height="10" fill="#0B3056" />
          <text x="80" y="103" textAnchor="middle" fill="#0B3056" fontSize="6.5" fontFamily="monospace" letterSpacing="1">{code.toUpperCase()}</text>
        </svg>
      );
  }
}
