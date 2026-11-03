import ARKit
import AVFoundation
import UIKit

/// Error codes shared with Android and the room scan (see pigeons/ar_measure.dart).
enum ArMeasureErrorCode: String {
  case cameraPermissionDenied = "camera_permission_denied"
  case unavailable
  case sessionFailed = "session_failed"
  case noFloor = "no_floor"
  case busy

  var error: PigeonError { PigeonError(code: rawValue, message: nil, details: nil) }
}

/// Pigeon host side: checks ARKit support and presents [ArMeasureViewController].
final class ArMeasureHost: ArMeasureApi {
  private var pending: ((Result<ArMeasureResult?, Error>) -> Void)?

  func checkAvailability(completion: @escaping (Result<ArAvailability, Error>) -> Void) {
    completion(.success(ARWorldTrackingConfiguration.isSupported ? .supported : .unsupported))
  }

  func measureFloor(
    request: ArMeasureRequest, completion: @escaping (Result<ArMeasureResult?, Error>) -> Void
  ) {
    guard pending == nil else {
      completion(.failure(ArMeasureErrorCode.busy.error))
      return
    }
    guard ARWorldTrackingConfiguration.isSupported else {
      completion(.failure(ArMeasureErrorCode.unavailable.error))
      return
    }
    pending = completion
    NativeScreen.withCameraAccess { granted in
      if granted {
        self.present(request)
      } else {
        self.finish(.failure(ArMeasureErrorCode.cameraPermissionDenied.error))
      }
    }
  }

  private func present(_ request: ArMeasureRequest) {
    guard let presenter = NativeScreen.topViewController() else {
      finish(.failure(ArMeasureErrorCode.sessionFailed.error))
      return
    }
    let controller = ArMeasureViewController(request: request) { [weak self] outcome in
      self?.finish(outcome)
    }
    controller.modalPresentationStyle = .fullScreen
    presenter.present(controller, animated: true)
  }

  private func finish(_ outcome: Result<ArMeasureResult?, Error>) {
    let callback = pending
    pending = nil
    callback?(outcome)
  }
}

/// Shared by the AR measure and room scan hosts.
enum NativeScreen {
  /// Asks for the camera when needed; [completion] runs on the main thread.
  static func withCameraAccess(_ completion: @escaping (Bool) -> Void) {
    switch AVCaptureDevice.authorizationStatus(for: .video) {
    case .authorized:
      completion(true)
    case .notDetermined:
      AVCaptureDevice.requestAccess(for: .video) { granted in
        DispatchQueue.main.async { completion(granted) }
      }
    default:
      completion(false)
    }
  }

  static func topViewController() -> UIViewController? {
    let window = UIApplication.shared.connectedScenes
      .compactMap { $0 as? UIWindowScene }
      .flatMap(\.windows)
      .first(where: \.isKeyWindow)
    var top = window?.rootViewController
    while let presented = top?.presentedViewController { top = presented }
    return top
  }
}

/// 2D point on the floor in world metres (x and z; ARKit's y axis is up).
struct PlanePoint {
  let x: Double
  let z: Double
}

/// Same polygon maths as Android's PolygonMath.kt.
enum PolygonMath {
  /// Shoelace area of the closed polygon [points].
  static func area(_ points: [PlanePoint]) -> Double {
    guard points.count >= 3 else { return 0 }
    var sum = 0.0
    for i in points.indices {
      let a = points[i]
      let b = points[(i + 1) % points.count]
      sum += a.x * b.z - b.x * a.z
    }
    return abs(sum) / 2
  }

  /// Edge lengths; [closed] adds the edge from the last point back to the first.
  static func edgeLengths(_ points: [PlanePoint], closed: Bool) -> [Double] {
    let count = max(closed ? points.count : points.count - 1, 0)
    return (0..<count).map { i in
      let a = points[i]
      let b = points[(i + 1) % points.count]
      return hypot(b.x - a.x, b.z - a.z)
    }
  }

  /// True when any two non-adjacent edges of the closed polygon cross or touch.
  static func isSelfIntersecting(_ points: [PlanePoint]) -> Bool {
    let n = points.count
    guard n >= 4 else { return false }
    for i in 0..<n {
      for j in (i + 1)..<n {
        // Skip edges sharing a vertex (consecutive, or first and last).
        if j == i + 1 || (i == 0 && j == n - 1) { continue }
        if segmentsIntersect(points[i], points[(i + 1) % n], points[j], points[(j + 1) % n]) {
          return true
        }
      }
    }
    return false
  }

  private static func segmentsIntersect(
    _ p1: PlanePoint, _ p2: PlanePoint, _ p3: PlanePoint, _ p4: PlanePoint
  ) -> Bool {
    let d1 = cross(p3, p4, p1)
    let d2 = cross(p3, p4, p2)
    let d3 = cross(p1, p2, p3)
    let d4 = cross(p1, p2, p4)
    if ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)) {
      return true
    }
    return (d1 == 0 && onSegment(p3, p4, p1)) || (d2 == 0 && onSegment(p3, p4, p2))
      || (d3 == 0 && onSegment(p1, p2, p3)) || (d4 == 0 && onSegment(p1, p2, p4))
  }

  private static func cross(_ a: PlanePoint, _ b: PlanePoint, _ c: PlanePoint) -> Double {
    (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x)
  }

  private static func onSegment(_ a: PlanePoint, _ b: PlanePoint, _ p: PlanePoint) -> Bool {
    min(a.x, b.x) <= p.x && p.x <= max(a.x, b.x) && min(a.z, b.z) <= p.z && p.z <= max(a.z, b.z)
  }
}

extension UIColor {
  /// ARGB int from ErpTokens (Flutter `Color.toARGB32()`).
  convenience init(argb: Int64) {
    let value = UInt32(truncatingIfNeeded: argb)
    self.init(
      red: CGFloat((value >> 16) & 0xFF) / 255,
      green: CGFloat((value >> 8) & 0xFF) / 255,
      blue: CGFloat(value & 0xFF) / 255,
      alpha: CGFloat((value >> 24) & 0xFF) / 255)
  }
}

/// Text placed at an edge's on-screen midpoint.
struct EdgeLabel {
  let text: String
  let at: CGPoint
}

/// Everything the overlay draws for one camera frame.
struct OverlayState {
  /// Screen position per placed point; nil when the point is behind the camera.
  let points: [CGPoint?]
  let closed: Bool
  let labels: [EdgeLabel]
  let reticleOnPlane: Bool
}

/// Transparent view above the camera: reticle, points, dimension lines and
/// per-edge lengths. Square corners (Atelier, radius 0); colours from Flutter.
final class MeasureOverlayView: UIView {
  var state: OverlayState? { didSet { setNeedsDisplay() } }

  private let colors: ArMeasureColors
  private let pointHalf: CGFloat = 5
  private let reticleHalf: CGFloat = 14
  private let labelPad: CGFloat = 4
  private let labelFont = UIFont.monospacedDigitSystemFont(ofSize: 13, weight: .regular)

  init(colors: ArMeasureColors) {
    self.colors = colors
    super.init(frame: .zero)
    isOpaque = false
    backgroundColor = .clear
    contentMode = .redraw
  }

  @available(*, unavailable)
  required init?(coder: NSCoder) { fatalError("init(coder:) is not supported") }

  override func draw(_ rect: CGRect) {
    guard let s = state, let ctx = UIGraphicsGetCurrentContext() else { return }
    let accent = UIColor(argb: colors.accentMeasure)
    drawReticle(ctx, onPlane: s.reticleOnPlane)

    let visible = s.points
    let all = visible.compactMap { $0 }
    if s.closed && visible.count >= 3 && all.count == visible.count {
      ctx.beginPath()
      ctx.addLines(between: all)
      ctx.closePath()
      // Same oak accent, translucent, so the measured floor stays visible.
      ctx.setFillColor(accent.withAlphaComponent(0.25).cgColor)
      ctx.fillPath()
    }

    ctx.setStrokeColor(accent.cgColor)
    ctx.setLineWidth(2)
    let edgeCount = max(s.closed ? visible.count : visible.count - 1, 0)
    for i in 0..<edgeCount {
      guard let a = visible[i], let b = visible[(i + 1) % visible.count] else { continue }
      ctx.strokeLineSegments(between: [a, b])
    }

    for p in all {
      let box = CGRect(x: p.x - pointHalf, y: p.y - pointHalf, width: pointHalf * 2, height: pointHalf * 2)
      ctx.setFillColor(accent.cgColor)
      ctx.fill(box)
      ctx.setStrokeColor(UIColor(argb: colors.onAction).cgColor)
      ctx.setLineWidth(1)
      ctx.stroke(box)
    }

    let attributes: [NSAttributedString.Key: Any] = [
      .font: labelFont,
      .foregroundColor: UIColor(argb: colors.textMain),
    ]
    for label in s.labels {
      let text = label.text as NSString
      let size = text.size(withAttributes: attributes)
      let box = CGRect(
        x: label.at.x - size.width / 2 - labelPad, y: label.at.y - size.height / 2 - labelPad,
        width: size.width + labelPad * 2, height: size.height + labelPad * 2)
      ctx.setFillColor(UIColor(argb: colors.surface).cgColor)
      ctx.fill(box)
      text.draw(
        at: CGPoint(x: label.at.x - size.width / 2, y: label.at.y - size.height / 2),
        withAttributes: attributes)
    }
  }

  private func drawReticle(_ ctx: CGContext, onPlane: Bool) {
    let c = CGPoint(x: bounds.midX, y: bounds.midY)
    ctx.setStrokeColor(UIColor(argb: onPlane ? colors.onAction : colors.textMuted).cgColor)
    ctx.setLineWidth(2)
    ctx.stroke(CGRect(x: c.x - reticleHalf, y: c.y - reticleHalf, width: reticleHalf * 2, height: reticleHalf * 2))
    ctx.strokeLineSegments(between: [
      CGPoint(x: c.x - pointHalf, y: c.y), CGPoint(x: c.x + pointHalf, y: c.y),
      CGPoint(x: c.x, y: c.y - pointHalf), CGPoint(x: c.x, y: c.y + pointHalf),
    ])
  }
}

extension UIButton {
  /// Atelier button look from Flutter's colours. Square corners: layers default to radius 0.
  func applyErpStyle(colors: ArMeasureColors, primary: Bool, enabled: Bool) {
    titleLabel?.font = .systemFont(ofSize: 15)
    isEnabled = enabled
    layer.borderWidth = 0
    let foreground: Int64
    if !enabled {
      backgroundColor = UIColor(argb: colors.disabledBackground)
      foreground = colors.disabledForeground
    } else if primary {
      backgroundColor = UIColor(argb: colors.action)
      foreground = colors.onAction
    } else {
      backgroundColor = UIColor(argb: colors.surface)
      layer.borderWidth = 1
      layer.borderColor = UIColor(argb: colors.borderStrong).cgColor
      foreground = colors.textMain
    }
    setTitleColor(UIColor(argb: foreground), for: .normal)
    setTitleColor(UIColor(argb: foreground), for: .disabled)
  }
}

/// Button with a ≥ 48pt tap target and horizontal padding (no deprecated insets).
final class PaddedButton: UIButton {
  override var intrinsicContentSize: CGSize {
    let size = super.intrinsicContentSize
    return CGSize(width: size.width + 32, height: max(size.height, 48))
  }
}

/// Native AR floor measure: find a horizontal plane, tap to place corners,
/// close the polygon and return area/perimeter/edges. Texts and colours come
/// from the Flutter request; this screen defines none of its own.
final class ArMeasureViewController: UIViewController, ARSessionDelegate {
  private let strings: ArMeasureStrings
  private let colors: ArMeasureColors
  private let onFinish: (Result<ArMeasureResult?, Error>) -> Void
  private let formatter = NumberFormatter()

  private let sceneView = ARSCNView()
  private let overlay: MeasureOverlayView
  private let areaText = UILabel()
  private let messageText = UILabel()
  private let undoButton = PaddedButton(type: .custom)
  private let closeButton = PaddedButton(type: .custom)
  private let doneButton = PaddedButton(type: .custom)
  private var displayLink: CADisplayLink?

  private var anchors: [ARAnchor] = []
  /// Floor height (world y) of the first point; later points must be on the same floor.
  private var referenceHeight: Float?
  private var closed = false
  private var everTracked = false
  private var finished = false
  private var transientMessage: String?
  private var transientUntil: CFTimeInterval = 0

  init(request: ArMeasureRequest, onFinish: @escaping (Result<ArMeasureResult?, Error>) -> Void) {
    strings = request.strings
    colors = request.colors
    self.onFinish = onFinish
    overlay = MeasureOverlayView(colors: request.colors)
    super.init(nibName: nil, bundle: nil)
    formatter.locale = Locale(identifier: strings.localeTag)
    formatter.numberStyle = .decimal
    formatter.minimumFractionDigits = 2
    formatter.maximumFractionDigits = 2
  }

  @available(*, unavailable)
  required init?(coder: NSCoder) { fatalError("init(coder:) is not supported") }

  override var supportedInterfaceOrientations: UIInterfaceOrientationMask { .portrait }

  override func viewDidLoad() {
    super.viewDidLoad()
    buildViews()
    sceneView.session.delegate = self
    render(message: strings.searchingPlane, isError: false, area: nil, canUndo: false, canClose: false, canFinish: false)
  }

  override func viewWillAppear(_ animated: Bool) {
    super.viewWillAppear(animated)
    let config = ARWorldTrackingConfiguration()
    config.planeDetection = [.horizontal]
    sceneView.session.run(config)
    let link = CADisplayLink(target: self, selector: #selector(tick))
    link.add(to: .main, forMode: .common)
    displayLink = link
  }

  override func viewWillDisappear(_ animated: Bool) {
    super.viewWillDisappear(animated)
    // Invalidating breaks the display link's strong reference to self.
    displayLink?.invalidate()
    displayLink = nil
    sceneView.session.pause()
  }

  func session(_ session: ARSession, didFailWithError error: Error) {
    finish(.failure(ArMeasureErrorCode.sessionFailed.error))
  }

  private func finish(_ outcome: Result<ArMeasureResult?, Error>) {
    guard !finished else { return }
    finished = true
    dismiss(animated: true) { self.onFinish(outcome) }
  }

  // MARK: Views

  private func buildViews() {
    let space: CGFloat = 16
    view.backgroundColor = UIColor(argb: colors.surface)
    sceneView.automaticallyUpdatesLighting = false
    overlay.addGestureRecognizer(UITapGestureRecognizer(target: self, action: #selector(handleTap(_:))))

    let cancel = PaddedButton(type: .custom)
    style(cancel, title: strings.cancel, primary: false, enabled: true)
    cancel.addTarget(self, action: #selector(cancelTapped), for: .touchUpInside)
    style(undoButton, title: strings.undo, primary: false, enabled: false)
    undoButton.addTarget(self, action: #selector(undo), for: .touchUpInside)
    style(closeButton, title: strings.closeShape, primary: false, enabled: false)
    closeButton.addTarget(self, action: #selector(closeShape), for: .touchUpInside)
    style(doneButton, title: strings.done, primary: true, enabled: false)
    doneButton.addTarget(self, action: #selector(complete), for: .touchUpInside)

    let caption = UILabel()
    caption.text = strings.areaLabel
    caption.textColor = UIColor(argb: colors.textMuted)
    caption.font = .systemFont(ofSize: 13)
    areaText.textColor = UIColor(argb: colors.textMain)
    areaText.font = .monospacedDigitSystemFont(ofSize: 44, weight: .bold)
    areaText.adjustsFontSizeToFitWidth = true
    messageText.font = .systemFont(ofSize: 15)
    messageText.numberOfLines = 0

    let row = UIStackView(arrangedSubviews: [undoButton, closeButton])
    row.axis = .horizontal
    row.spacing = space
    row.distribution = .fillEqually
    let stack = UIStackView(arrangedSubviews: [caption, areaText, messageText, row, doneButton])
    stack.axis = .vertical
    stack.spacing = space / 2
    stack.setCustomSpacing(space, after: messageText)
    stack.setCustomSpacing(space, after: row)

    let panel = UIView()
    panel.backgroundColor = UIColor(argb: colors.surface)

    for v in [sceneView, overlay, panel, cancel] as [UIView] {
      v.translatesAutoresizingMaskIntoConstraints = false
      view.addSubview(v)
    }
    stack.translatesAutoresizingMaskIntoConstraints = false
    panel.addSubview(stack)
    let guide = view.safeAreaLayoutGuide
    NSLayoutConstraint.activate([
      sceneView.topAnchor.constraint(equalTo: view.topAnchor),
      sceneView.bottomAnchor.constraint(equalTo: view.bottomAnchor),
      sceneView.leadingAnchor.constraint(equalTo: view.leadingAnchor),
      sceneView.trailingAnchor.constraint(equalTo: view.trailingAnchor),
      overlay.topAnchor.constraint(equalTo: view.topAnchor),
      overlay.bottomAnchor.constraint(equalTo: view.bottomAnchor),
      overlay.leadingAnchor.constraint(equalTo: view.leadingAnchor),
      overlay.trailingAnchor.constraint(equalTo: view.trailingAnchor),
      cancel.topAnchor.constraint(equalTo: guide.topAnchor, constant: space),
      cancel.leadingAnchor.constraint(equalTo: guide.leadingAnchor, constant: space),
      panel.leadingAnchor.constraint(equalTo: view.leadingAnchor),
      panel.trailingAnchor.constraint(equalTo: view.trailingAnchor),
      panel.bottomAnchor.constraint(equalTo: view.bottomAnchor),
      stack.topAnchor.constraint(equalTo: panel.topAnchor, constant: space),
      stack.leadingAnchor.constraint(equalTo: guide.leadingAnchor, constant: space),
      stack.trailingAnchor.constraint(equalTo: guide.trailingAnchor, constant: -space),
      stack.bottomAnchor.constraint(equalTo: guide.bottomAnchor, constant: -space),
      doneButton.heightAnchor.constraint(equalToConstant: 52),
    ])
  }

  private func style(_ button: UIButton, title: String?, primary: Bool, enabled: Bool) {
    if let title { button.setTitle(title, for: .normal) }
    button.applyErpStyle(colors: colors, primary: primary, enabled: enabled)
  }

  private func render(
    message: String, isError: Bool, area: Double?, canUndo: Bool, canClose: Bool, canFinish: Bool
  ) {
    areaText.text = format(area ?? 0, unit: strings.unitSquareMeter)
    messageText.text = message
    messageText.textColor = UIColor(argb: isError ? colors.danger : colors.textMain)
    if undoButton.isEnabled != canUndo { style(undoButton, title: nil, primary: false, enabled: canUndo) }
    if closeButton.isEnabled != canClose { style(closeButton, title: nil, primary: false, enabled: canClose) }
    if doneButton.isEnabled != canFinish { style(doneButton, title: nil, primary: true, enabled: canFinish) }
  }

  private func format(_ value: Double, unit: String) -> String {
    let number = formatter.string(from: NSNumber(value: value)) ?? String(format: "%.2f", value)
    return "\(number) \(unit)"
  }

  // MARK: Actions

  @objc private func cancelTapped() { finish(.success(nil)) }

  @objc private func handleTap(_ gesture: UITapGestureRecognizer) {
    guard !closed, let frame = sceneView.session.currentFrame, isTracking(frame.camera) else { return }
    guard let hit = floorHit(at: gesture.location(in: sceneView)) else {
      showTransient(strings.notOnPlane)
      return
    }
    if referenceHeight == nil { referenceHeight = hit.worldTransform.columns.3.y }
    let anchor = ARAnchor(transform: hit.worldTransform)
    sceneView.session.add(anchor: anchor)
    anchors.append(anchor)
  }

  @objc private func undo() {
    if closed {
      closed = false
    } else if let last = anchors.popLast() {
      sceneView.session.remove(anchor: last)
    }
    if anchors.isEmpty { referenceHeight = nil }
  }

  @objc private func closeShape() {
    guard !closed, anchors.count >= 3 else { return }
    if PolygonMath.isSelfIntersecting(planePoints(currentAnchors())) {
      showTransient(strings.selfIntersecting)
      return
    }
    closed = true
  }

  @objc private func complete() {
    guard closed else { return }
    let points = planePoints(currentAnchors())
    let edges = PolygonMath.edgeLengths(points, closed: true)
    finish(
      .success(
        ArMeasureResult(
          areaSqm: PolygonMath.area(points),
          perimeterM: edges.reduce(0, +),
          edgeLengthsM: edges,
          vertexCount: Int64(edges.count))))
  }

  private func showTransient(_ message: String) {
    transientMessage = message
    transientUntil = CACurrentMediaTime() + 2.5
  }

  // MARK: Geometry

  /// TrackingState is Equatable only from iOS 16; match the case instead.
  private func isTracking(_ camera: ARCamera) -> Bool {
    if case .normal = camera.trackingState { return true }
    return false
  }

  /// Raycast onto an upward-facing horizontal plane on the reference floor.
  private func floorHit(at point: CGPoint) -> ARRaycastResult? {
    guard
      let query = sceneView.raycastQuery(from: point, allowing: .existingPlaneGeometry, alignment: .horizontal)
    else { return nil }
    return sceneView.session.raycast(query).first { hit in
      // Plane normal is the hit's y axis; skip ceilings and table undersides.
      let upward = hit.worldTransform.columns.1.y > 0.9
      let sameFloor = referenceHeight.map { abs(hit.worldTransform.columns.3.y - $0) < 0.05 } ?? true
      return upward && sameFloor
    }
  }

  /// Placed anchors with the latest transforms ARKit has refined for them.
  private func currentAnchors() -> [ARAnchor] {
    guard let frame = sceneView.session.currentFrame else { return anchors }
    let latest = Dictionary(uniqueKeysWithValues: frame.anchors.map { ($0.identifier, $0) })
    return anchors.map { latest[$0.identifier] ?? $0 }
  }

  private func planePoints(_ anchors: [ARAnchor]) -> [PlanePoint] {
    anchors.map { PlanePoint(x: Double($0.transform.columns.3.x), z: Double($0.transform.columns.3.z)) }
  }

  private func project(_ anchor: ARAnchor, camera: ARCamera) -> CGPoint? {
    let world = anchor.transform.columns.3
    // Camera looks down its -z axis; points at or behind it are not drawn.
    let local = simd_mul(simd_inverse(camera.transform), world)
    guard local.z < 0 else { return nil }
    let orientation = view.window?.windowScene?.interfaceOrientation ?? .portrait
    return camera.projectPoint(
      simd_float3(world.x, world.y, world.z), orientation: orientation, viewportSize: sceneView.bounds.size)
  }

  // MARK: Frame loop

  @objc private func tick() {
    guard let frame = sceneView.session.currentFrame else { return }
    let camera = frame.camera
    let tracking = isTracking(camera)
    if tracking { everTracked = true }

    let placed = currentAnchors()
    let screen = placed.map { project($0, camera: camera) }
    let points = planePoints(placed)
    let edges = PolygonMath.edgeLengths(points, closed: closed)
    let labels = edges.enumerated().compactMap { i, length -> EdgeLabel? in
      guard let a = screen[i], let b = screen[(i + 1) % screen.count] else { return nil }
      return EdgeLabel(text: format(length, unit: strings.unitMeter), at: CGPoint(x: (a.x + b.x) / 2, y: (a.y + b.y) / 2))
    }
    let planeFound = tracking && frame.anchors.contains { ($0 as? ARPlaneAnchor)?.alignment == .horizontal }
    let center = CGPoint(x: sceneView.bounds.midX, y: sceneView.bounds.midY)
    let reticleOnPlane = tracking && floorHit(at: center) != nil

    let transient = CACurrentMediaTime() < transientUntil ? transientMessage : nil
    let message: String
    var isError = false
    if !tracking && everTracked {
      message = strings.trackingLost
      isError = true
    } else if let transient {
      message = transient
      isError = true
    } else if !planeFound && anchors.isEmpty {
      message = strings.searchingPlane
    } else if anchors.isEmpty {
      message = strings.tapToPlace
    } else if anchors.count < 3 {
      message = strings.addMorePoints
    } else if !closed {
      message = strings.readyToClose
    } else {
      message = strings.closedHint
    }

    overlay.state = OverlayState(points: screen, closed: closed, labels: labels, reticleOnPlane: reticleOnPlane)
    render(
      message: message,
      isError: isError,
      area: points.count >= 3 ? PolygonMath.area(points) : nil,
      canUndo: !anchors.isEmpty,
      canClose: !closed && anchors.count >= 3,
      canFinish: closed)
  }
}
