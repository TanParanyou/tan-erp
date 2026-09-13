"use client";

import React, { useEffect, useRef, useState, useCallback } from "react";
import * as THREE from "three";
import { useTranslations } from "next-intl";
import {
  IconBox3D,
  IconClose,
  IconZoomIn,
  IconZoomOut,
} from "@/components/common/Icons";

export interface ThreeBoxVisualizerProps {
  /** Width in millimeters (X-axis) */
  widthMm: number;
  /** Length / Depth in millimeters (Z-axis) */
  lengthMm: number;
  /** Height in millimeters (Y-axis) */
  heightMm: number;
  /** Title / Label to display above visualizer */
  title?: string;
  /** Height of the 3D canvas viewport in pixels (default 300) */
  viewportHeight?: number;
  /** Optional custom CSS classes for the container */
  className?: string;
  /** Callback when close button is clicked (if provided, close button will render) */
  onClose?: () => void;
  /** Optional warning message if measurements are incomplete */
  warningMessage?: string;
}

export function ThreeBoxVisualizer({
  widthMm,
  lengthMm,
  heightMm,
  title,
  viewportHeight = 300,
  className = "",
  onClose,
  warningMessage,
}: ThreeBoxVisualizerProps) {
  const t = useTranslations("surveys");
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);

  // Scene references
  const sceneRef = useRef<THREE.Scene | null>(null);
  const cameraRef = useRef<THREE.PerspectiveCamera | null>(null);
  const rendererRef = useRef<THREE.WebGLRenderer | null>(null);
  const boxMeshRef = useRef<THREE.Mesh | null>(null);
  const edgesLineRef = useRef<THREE.LineSegments | null>(null);
  const gridHelperRef = useRef<THREE.GridHelper | null>(null);
  const animFrameIdRef = useRef<number | null>(null);

  // Orbit state
  const isDraggingRef = useRef(false);
  const prevMouseRef = useRef({ x: 0, y: 0 });
  const sphericalRef = useRef({ radius: 8, theta: Math.PI / 4, phi: Math.PI / 3 });

  // Zoom Slider state (Normalized percentage from 10% to 200% where 100% is default auto-fit)
  const [zoomPercent, setZoomPercent] = useState<number>(100);
  const [activeView, setActiveView] = useState<"iso" | "top" | "front" | "side">("iso");

  // Convert mm to Three.js meter units (1 unit = 1 meter = 1000 mm)
  const widthM = Math.max(0.1, widthMm / 1000);
  const lengthM = Math.max(0.1, lengthMm / 1000);
  const heightM = Math.max(0.1, heightMm / 1000);

  // Base fit radius based on room dimensions
  const maxDim = Math.max(widthM, lengthM, heightM);
  const baseRadius = Math.max(5, maxDim * 2.2);

  // Update Camera Position based on Spherical coordinates and focus on box center
  const updateCameraPosition = useCallback((targetHeightM: number = heightM) => {
    if (!cameraRef.current) return;
    const { radius, theta, phi } = sphericalRef.current;
    const centerY = targetHeightM / 2;
    const x = radius * Math.sin(phi) * Math.sin(theta);
    const y = centerY + radius * Math.cos(phi);
    const z = radius * Math.sin(phi) * Math.cos(theta);
    cameraRef.current.position.set(x, y, z);
    cameraRef.current.lookAt(0, centerY, 0);
  }, [heightM]);

  // Handle Zoom Slider Drag change
  const handleZoomSliderChange = (newPercent: number) => {
    setZoomPercent(newPercent);
    // When percent is 100%, radius is baseRadius
    // Higher percent = closer (smaller radius)
    // Lower percent = further (larger radius)
    const factor = 100 / Math.max(10, newPercent);
    sphericalRef.current.radius = Math.max(2, Math.min(30, baseRadius * factor));
    updateCameraPosition(heightM);
  };

  // Step Zoom In / Out buttons (+ / - 15%)
  const handleStepZoom = (delta: number) => {
    const nextVal = Math.max(20, Math.min(200, zoomPercent + delta));
    handleZoomSliderChange(nextVal);
  };

  // Preset Views (Isometric, Top, Front, Side)
  const handleSetView = (view: "iso" | "top" | "front" | "side") => {
    setActiveView(view);
    if (view === "iso") {
      sphericalRef.current.theta = Math.PI / 4;
      sphericalRef.current.phi = Math.PI / 3;
    } else if (view === "top") {
      sphericalRef.current.theta = 0;
      sphericalRef.current.phi = 0.05; // almost top down
    } else if (view === "front") {
      sphericalRef.current.theta = 0;
      sphericalRef.current.phi = Math.PI / 2 - 0.05;
    } else if (view === "side") {
      sphericalRef.current.theta = Math.PI / 2;
      sphericalRef.current.phi = Math.PI / 2 - 0.05;
    }
    updateCameraPosition(heightM);
  };

  // Reset camera view
  const handleResetCamera = () => {
    sphericalRef.current.theta = Math.PI / 4;
    sphericalRef.current.phi = Math.PI / 3;
    setZoomPercent(100);
    sphericalRef.current.radius = baseRadius;
    setActiveView("iso");
    updateCameraPosition(heightM);
  };

  // Initialize Three.js Scene
  useEffect(() => {
    const canvas = canvasRef.current;
    const container = containerRef.current;
    if (!canvas || !container) return;

    const width = container.clientWidth || 400;
    const height = viewportHeight;

    // 1. Scene
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf8fafc); // Light slate canvas
    sceneRef.current = scene;

    // 2. Camera
    const camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 100);
    cameraRef.current = camera;
    sphericalRef.current.radius = baseRadius;
    updateCameraPosition(heightM);

    // 3. Renderer with antialias (Gracefully catch WebGL unavailable in JSDOM)
    let renderer: THREE.WebGLRenderer | null = null;
    try {
      renderer = new THREE.WebGLRenderer({
        canvas,
        antialias: true,
        alpha: true,
        powerPreference: "default",
      });
      renderer.setSize(width, height, false);
      renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
      rendererRef.current = renderer;
    } catch {
      renderer = null;
      rendererRef.current = null;
    }

    // 4. Lights
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.85);
    scene.add(ambientLight);

    const dirLight = new THREE.DirectionalLight(0xffffff, 0.7);
    dirLight.position.set(6, 12, 8);
    scene.add(dirLight);

    // 5. Floor Grid (Architectural blueprint style)
    const initialGridSize = Math.max(10, Math.ceil(Math.max(widthM, lengthM) * 1.6));
    const gridHelper = new THREE.GridHelper(initialGridSize, initialGridSize * 2, 0x0b3056, 0xcbd5e1);
    gridHelper.position.y = 0;
    scene.add(gridHelper);
    gridHelperRef.current = gridHelper;

    // 6. Box & Edges Mesh
    const geometry = new THREE.BoxGeometry(widthM, heightM, lengthM);
    // Translucent navy atelier material
    const material = new THREE.MeshLambertMaterial({
      color: 0x0b3056,
      transparent: true,
      opacity: 0.22,
      depthWrite: false,
    });
    const boxMesh = new THREE.Mesh(geometry, material);
    boxMesh.position.set(0, heightM / 2, 0);
    scene.add(boxMesh);
    boxMeshRef.current = boxMesh;

    // Sharp outer wireframe lines
    const edgesGeom = new THREE.EdgesGeometry(geometry);
    const edgesMat = new THREE.LineBasicMaterial({
      color: 0x0b3056,
      linewidth: 2,
    });
    const edgesLine = new THREE.LineSegments(edgesGeom, edgesMat);
    edgesLine.position.set(0, heightM / 2, 0);
    scene.add(edgesLine);
    edgesLineRef.current = edgesLine;

    // 7. Render loop
    const render = () => {
      if (rendererRef.current) {
        rendererRef.current.render(scene, camera);
        animFrameIdRef.current = requestAnimationFrame(render);
      }
    };
    if (renderer) {
      render();
    }

    // 8. Resize Observer (Safely guarded for SSR/JSDOM)
    let resizeObserver: ResizeObserver | null = null;
    if (typeof ResizeObserver !== "undefined") {
      resizeObserver = new ResizeObserver((entries) => {
        for (const entry of entries) {
          const newWidth = entry.contentRect.width;
          if (newWidth > 0 && cameraRef.current && rendererRef.current) {
            cameraRef.current.aspect = newWidth / viewportHeight;
            cameraRef.current.updateProjectionMatrix();
            rendererRef.current.setSize(newWidth, viewportHeight, false);
          }
        }
      });
      resizeObserver.observe(container);
    }

    // Cleanup
    return () => {
      if (animFrameIdRef.current) {
        cancelAnimationFrame(animFrameIdRef.current);
      }
      if (resizeObserver) {
        resizeObserver.disconnect();
      }
      geometry.dispose();
      material.dispose();
      edgesGeom.dispose();
      edgesMat.dispose();
      if (rendererRef.current) {
        rendererRef.current.dispose();
      }
    };
  }, [viewportHeight, updateCameraPosition, baseRadius, heightM, widthM, lengthM]);

  // Update Geometry when dimensions change
  useEffect(() => {
    if (!boxMeshRef.current || !edgesLineRef.current || !sceneRef.current) return;

    const oldBoxGeom = boxMeshRef.current.geometry;
    const oldEdgesGeom = edgesLineRef.current.geometry;

    const newBoxGeom = new THREE.BoxGeometry(widthM, heightM, lengthM);
    const newEdgesGeom = new THREE.EdgesGeometry(newBoxGeom);

    boxMeshRef.current.geometry = newBoxGeom;
    boxMeshRef.current.position.set(0, heightM / 2, 0);

    edgesLineRef.current.geometry = newEdgesGeom;
    edgesLineRef.current.position.set(0, heightM / 2, 0);

    // Adjust camera distance to fit box well
    const factor = 100 / Math.max(10, zoomPercent);
    sphericalRef.current.radius = Math.max(2, Math.min(30, baseRadius * factor));
    updateCameraPosition(heightM);

    // Scale Floor Grid to gracefully accommodate larger rooms
    if (gridHelperRef.current) {
      const neededGridScale = Math.max(1, (Math.max(widthM, lengthM) * 1.5) / 10);
      gridHelperRef.current.scale.set(neededGridScale, 1, neededGridScale);
    }

    oldBoxGeom.dispose();
    oldEdgesGeom.dispose();
  }, [widthM, lengthM, heightM, updateCameraPosition, baseRadius, zoomPercent]);

  // Mouse / Touch Orbit Event Handlers
  const handlePointerDown = (e: React.PointerEvent) => {
    isDraggingRef.current = true;
    prevMouseRef.current = { x: e.clientX, y: e.clientY };
    (e.target as HTMLElement).setPointerCapture(e.pointerId);
  };

  const handlePointerMove = (e: React.PointerEvent) => {
    if (!isDraggingRef.current) return;
    const dx = e.clientX - prevMouseRef.current.x;
    const dy = e.clientY - prevMouseRef.current.y;
    prevMouseRef.current = { x: e.clientX, y: e.clientY };

    const speed = 0.006;
    sphericalRef.current.theta -= dx * speed;
    // Constrain phi to avoid flipping camera over poles
    sphericalRef.current.phi = Math.max(
      0.05,
      Math.min(Math.PI / 2 - 0.05, sphericalRef.current.phi - dy * speed)
    );

    updateCameraPosition(heightM);
  };

  const handlePointerUp = (e: React.PointerEvent) => {
    isDraggingRef.current = false;
    try {
      (e.target as HTMLElement).releasePointerCapture(e.pointerId);
    } catch {
      // Ignore if pointer capture already lost
    }
  };

  // Wheel Zoom also syncs slider percentage
  const handleWheel = (e: React.WheelEvent) => {
    e.preventDefault();
    const zoomDelta = e.deltaY > 0 ? -10 : 10;
    setZoomPercent((prev) => {
      const next = Math.max(20, Math.min(200, prev + zoomDelta));
      const factor = 100 / next;
      sphericalRef.current.radius = Math.max(2, Math.min(30, baseRadius * factor));
      updateCameraPosition(heightM);
      return next;
    });
  };

  return (
    <div
      ref={containerRef}
      className={`relative border border-erp-border bg-slate-50 overflow-hidden ${className}`}
    >
      {/* Top Header Bar */}
      <div className="px-3 py-2 bg-erp-surface border-b border-erp-border flex flex-wrap items-center justify-between gap-2">
        {/* Title & Dimension pills */}
        <div className="flex items-center gap-2">
          <IconBox3D size={16} className="text-erp-navy shrink-0" strokeWidth={2} />
          <span className="text-xs font-bold text-erp-navy truncate">
            {title ? `${title} (3D)` : t("visualizer3D.toggleOpen")}
          </span>
          <span className="hidden sm:inline text-[11px] font-mono font-semibold text-erp-text-muted bg-erp-surface-subtle px-1.5 py-0.5 border border-erp-border">
            {Math.round(widthMm).toLocaleString()} × {Math.round(lengthMm).toLocaleString()} × {Math.round(heightMm).toLocaleString()} mm
          </span>
        </div>

        {/* Preset Angle Buttons & Actions */}
        <div className="flex items-center gap-1.5">
          <div className="hidden md:flex items-center border border-erp-border bg-erp-surface-subtle">
            <button
              type="button"
              onClick={() => handleSetView("iso")}
              className={`text-[11px] px-2 py-0.5 font-medium transition-colors cursor-pointer ${
                activeView === "iso" ? "bg-erp-navy text-white" : "text-erp-text-body hover:bg-erp-surface"
              }`}
            >
              {t("visualizer3D.views.isometric")}
            </button>
            <button
              type="button"
              onClick={() => handleSetView("top")}
              className={`text-[11px] px-2 py-0.5 font-medium border-l border-erp-border transition-colors cursor-pointer ${
                activeView === "top" ? "bg-erp-navy text-white" : "text-erp-text-body hover:bg-erp-surface"
              }`}
            >
              {t("visualizer3D.views.top")}
            </button>
            <button
              type="button"
              onClick={() => handleSetView("front")}
              className={`text-[11px] px-2 py-0.5 font-medium border-l border-erp-border transition-colors cursor-pointer ${
                activeView === "front" ? "bg-erp-navy text-white" : "text-erp-text-body hover:bg-erp-surface"
              }`}
            >
              {t("visualizer3D.views.front")}
            </button>
            <button
              type="button"
              onClick={() => handleSetView("side")}
              className={`text-[11px] px-2 py-0.5 font-medium border-l border-erp-border transition-colors cursor-pointer ${
                activeView === "side" ? "bg-erp-navy text-white" : "text-erp-text-body hover:bg-erp-surface"
              }`}
            >
              {t("visualizer3D.views.side")}
            </button>
          </div>

          <button
            type="button"
            onClick={handleResetCamera}
            className="text-[11px] px-2 py-1 bg-erp-surface hover:bg-erp-surface-subtle text-erp-text-body border border-erp-border transition-colors font-medium cursor-pointer"
            title={t("visualizer3D.resetCamera")}
          >
            {t("visualizer3D.resetCamera")}
          </button>
          {onClose && (
            <button
              type="button"
              onClick={onClose}
              className="p-1 text-erp-text-muted hover:text-erp-danger hover:bg-erp-surface-subtle transition-colors cursor-pointer"
              title={t("visualizer3D.toggleClose")}
              aria-label={t("visualizer3D.toggleClose")}
            >
              <IconClose size={16} strokeWidth={2} />
            </button>
          )}
        </div>
      </div>

      {/* 3D WebGL Canvas */}
      <div className="relative">
        <canvas
          ref={canvasRef}
          style={{ height: `${viewportHeight}px`, width: "100%", display: "block" }}
          onPointerDown={handlePointerDown}
          onPointerMove={handlePointerMove}
          onPointerUp={handlePointerUp}
          onPointerCancel={handlePointerUp}
          onWheel={handleWheel}
          className="cursor-grab active:cursor-grabbing touch-none select-none bg-slate-50"
        />

        {/* Warning Notice overlay if incomplete dimensions */}
        {warningMessage && (
          <div className="absolute top-2 left-2 right-2 z-10 p-1.5 bg-amber-50/95 border border-amber-200 text-amber-800 text-[11px] text-center shadow-xs">
            {warningMessage}
          </div>
        )}
      </div>

      {/* Bottom Floating Control Bar: Drag Zoom Slider & Hint */}
      <div className="px-3 py-2 bg-erp-surface border-t border-erp-border flex flex-wrap items-center justify-between gap-3 text-xs">
        {/* Helper Hint */}
        <div className="text-[11px] text-erp-text-muted">
          <span>{t("visualizer3D.orbitHint")}</span>
        </div>

        {/* Dedicated Drag Zoom Controls */}
        <div className="flex items-center gap-2 w-full sm:w-auto justify-end">
          <span className="text-[11px] font-bold text-erp-navy uppercase">
            {t("visualizer3D.zoomLabel")}
          </span>

          {/* Zoom Out Button */}
          <button
            type="button"
            onClick={() => handleStepZoom(-15)}
            className="p-1 text-erp-text-main bg-erp-surface hover:bg-erp-surface-subtle border border-erp-border cursor-pointer"
            title={t("visualizer3D.zoomOut")}
            aria-label={t("visualizer3D.zoomOut")}
          >
            <IconZoomOut size={14} strokeWidth={2} />
          </button>

          {/* Drag Slider (Range Input) */}
          <div className="flex items-center gap-1.5">
            <input
              type="range"
              min="20"
              max="200"
              step="5"
              value={zoomPercent}
              onChange={(e) => handleZoomSliderChange(Number(e.target.value))}
              aria-label={t("visualizer3D.zoomLabel")}
              className="w-28 sm:w-36 h-2 bg-slate-200 accent-erp-navy cursor-pointer"
            />
            <span className="font-mono text-[11px] text-erp-navy font-bold w-10 text-right">
              {zoomPercent}%
            </span>
          </div>

          {/* Zoom In Button */}
          <button
            type="button"
            onClick={() => handleStepZoom(15)}
            className="p-1 text-erp-text-main bg-erp-surface hover:bg-erp-surface-subtle border border-erp-border cursor-pointer"
            title={t("visualizer3D.zoomIn")}
            aria-label={t("visualizer3D.zoomIn")}
          >
            <IconZoomIn size={14} strokeWidth={2} />
          </button>
        </div>
      </div>
    </div>
  );
}
