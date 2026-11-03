import RoomPlan
import UIKit

/// Pigeon host side: RoomPlan room scan (LiDAR devices, iOS 17+ for floors).
final class RoomScanHost: RoomScanApi {
  private var pending: ((Result<RoomScanResult?, Error>) -> Void)?

  func isSupported() throws -> Bool {
    guard #available(iOS 17.0, *) else { return false }
    return RoomCaptureSession.isSupported
  }

  func scanRoom(
    request: RoomScanRequest, completion: @escaping (Result<RoomScanResult?, Error>) -> Void
  ) {
    guard pending == nil else {
      completion(.failure(ArMeasureErrorCode.busy.error))
      return
    }
    guard #available(iOS 17.0, *), RoomCaptureSession.isSupported else {
      completion(.failure(ArMeasureErrorCode.unavailable.error))
      return
    }
    pending = completion
    NativeScreen.withCameraAccess { granted in
      guard granted else {
        self.finish(.failure(ArMeasureErrorCode.cameraPermissionDenied.error))
        return
      }
      guard let presenter = NativeScreen.topViewController() else {
        self.finish(.failure(ArMeasureErrorCode.sessionFailed.error))
        return
      }
      let controller = RoomScanViewController(request: request) { [weak self] outcome in
        self?.finish(outcome)
      }
      controller.modalPresentationStyle = .fullScreen
      presenter.present(controller, animated: true)
    }
  }

  private func finish(_ outcome: Result<RoomScanResult?, Error>) {
    let callback = pending
    pending = nil
    callback?(outcome)
  }
}

/// Native room scan: RoomPlan's own capture view and coaching, plus cancel/done.
/// Done stops the scan; once RoomPlan shows the processed room, done returns it.
@available(iOS 17.0, *)
final class RoomScanViewController: UIViewController, RoomCaptureViewDelegate {
  private enum Phase {
    case scanning, processing, review
  }

  private let strings: RoomScanRequest
  private let onFinish: (Result<RoomScanResult?, Error>) -> Void
  private lazy var captureView = RoomCaptureView(frame: .zero)
  private let cancelButton = PaddedButton(type: .custom)
  private let doneButton = PaddedButton(type: .custom)
  private var phase = Phase.scanning
  private var room: CapturedRoom?
  private var finished = false

  init(request: RoomScanRequest, onFinish: @escaping (Result<RoomScanResult?, Error>) -> Void) {
    strings = request
    self.onFinish = onFinish
    super.init(nibName: nil, bundle: nil)
  }

  required init?(coder: NSCoder) { fatalError("init(coder:) is not supported") }

  override var supportedInterfaceOrientations: UIInterfaceOrientationMask { .portrait }

  override func viewDidLoad() {
    super.viewDidLoad()
    let colors = strings.colors
    let space: CGFloat = 16
    view.backgroundColor = UIColor(argb: colors.surface)
    captureView.delegate = self

    cancelButton.setTitle(strings.cancel, for: .normal)
    cancelButton.applyErpStyle(colors: colors, primary: false, enabled: true)
    cancelButton.addTarget(self, action: #selector(cancelTapped), for: .touchUpInside)
    doneButton.setTitle(strings.done, for: .normal)
    doneButton.addTarget(self, action: #selector(doneTapped), for: .touchUpInside)
    renderButtons()

    let row = UIStackView(arrangedSubviews: [cancelButton, doneButton])
    row.axis = .horizontal
    row.spacing = space
    row.distribution = .fillEqually
    let panel = UIView()
    panel.backgroundColor = UIColor(argb: colors.surface)

    for v in [captureView, panel] as [UIView] {
      v.translatesAutoresizingMaskIntoConstraints = false
      view.addSubview(v)
    }
    row.translatesAutoresizingMaskIntoConstraints = false
    panel.addSubview(row)
    let guide = view.safeAreaLayoutGuide
    NSLayoutConstraint.activate([
      captureView.topAnchor.constraint(equalTo: view.topAnchor),
      captureView.leadingAnchor.constraint(equalTo: view.leadingAnchor),
      captureView.trailingAnchor.constraint(equalTo: view.trailingAnchor),
      captureView.bottomAnchor.constraint(equalTo: panel.topAnchor),
      panel.leadingAnchor.constraint(equalTo: view.leadingAnchor),
      panel.trailingAnchor.constraint(equalTo: view.trailingAnchor),
      panel.bottomAnchor.constraint(equalTo: view.bottomAnchor),
      row.topAnchor.constraint(equalTo: panel.topAnchor, constant: space),
      row.leadingAnchor.constraint(equalTo: guide.leadingAnchor, constant: space),
      row.trailingAnchor.constraint(equalTo: guide.trailingAnchor, constant: -space),
      row.bottomAnchor.constraint(equalTo: guide.bottomAnchor, constant: -space),
      row.heightAnchor.constraint(equalToConstant: 52),
    ])
  }

  override func viewDidAppear(_ animated: Bool) {
    super.viewDidAppear(animated)
    if phase == .scanning {
      captureView.captureSession.run(configuration: RoomCaptureSession.Configuration())
    }
  }

  override func viewWillDisappear(_ animated: Bool) {
    super.viewWillDisappear(animated)
    if phase == .scanning { captureView.captureSession.stop() }
  }

  // MARK: RoomCaptureViewDelegate

  func captureView(shouldPresent roomDataForProcessing: CapturedRoomData, error: Error?) -> Bool {
    guard error == nil else {
      finish(.failure(ArMeasureErrorCode.sessionFailed.error))
      return false
    }
    return true
  }

  func captureView(didPresent processedResult: CapturedRoom, error: Error?) {
    guard error == nil else {
      finish(.failure(ArMeasureErrorCode.sessionFailed.error))
      return
    }
    room = processedResult
    phase = .review
    renderButtons()
  }

  // MARK: Actions

  @objc private func cancelTapped() { finish(.success(nil)) }

  @objc private func doneTapped() {
    switch phase {
    case .scanning:
      phase = .processing
      renderButtons()
      captureView.captureSession.stop()
    case .processing:
      break
    case .review:
      guard let room, let result = RoomDimensions.measure(room) else {
        finish(.failure(ArMeasureErrorCode.noFloor.error))
        return
      }
      finish(.success(result))
    }
  }

  private func renderButtons() {
    doneButton.applyErpStyle(colors: strings.colors, primary: true, enabled: phase != .processing)
  }

  private func finish(_ outcome: Result<RoomScanResult?, Error>) {
    guard !finished else { return }
    finished = true
    dismiss(animated: true) { self.onFinish(outcome) }
  }
}

/// Room dimensions from a processed RoomPlan scan.
@available(iOS 17.0, *)
enum RoomDimensions {
  /// Nil when RoomPlan found no floor. Area sums every floor; width/length are
  /// the largest floor's bounding box (width ≤ length); height is the tallest wall;
  /// doors, windows and open passages are listed with their width and height.
  static func measure(_ room: CapturedRoom) -> RoomScanResult? {
    var total = 0.0
    var largest: (area: Double, sides: [Double])?
    for floor in room.floors {
      let (points, sides) = planar(floor)
      let area = PolygonMath.area(points)
      total += area
      if area > (largest?.area ?? -1) { largest = (area, sides.sorted()) }
    }
    guard let largest, total > 0 else { return nil }
    return RoomScanResult(
      floorAreaSqm: total,
      widthM: largest.sides[0],
      lengthM: largest.sides[1],
      ceilingHeightM: room.walls.map { Double($0.dimensions.y) }.max(),
      grossWallAreaSqm: room.walls.reduce(0) { $0 + Double($1.dimensions.x * $1.dimensions.y) },
      openings: openings(room.doors, .door) + openings(room.windows, .window)
        + openings(room.openings, .opening))
  }

  /// Width is the surface's x size, height its y size.
  private static func openings(_ surfaces: [CapturedRoom.Surface], _ kind: RoomOpeningKind)
    -> [RoomOpening]
  {
    surfaces.map {
      RoomOpening(kind: kind, widthM: Double($0.dimensions.x), heightM: Double($0.dimensions.y))
    }
  }

  /// Floor corners in the floor's own plane: the two local axes it spans most.
  private static func planar(_ floor: CapturedRoom.Surface) -> ([PlanePoint], [Double]) {
    let corners = floor.polygonCorners
    guard corners.count >= 3 else {
      // No polygon: RoomPlan reports a rectangle through the surface size.
      let size = [floor.dimensions.x, floor.dimensions.y, floor.dimensions.z].map(Double.init)
        .sorted(by: >)
      let (a, b) = (size[0], size[1])
      let points = [
        PlanePoint(x: 0, z: 0), PlanePoint(x: a, z: 0), PlanePoint(x: a, z: b), PlanePoint(x: 0, z: b),
      ]
      return (points, [a, b])
    }
    let spans = (0..<3).map { axis -> (axis: Int, span: Double) in
      let values = corners.map { Double($0[axis]) }
      return (axis, (values.max() ?? 0) - (values.min() ?? 0))
    }.sorted { $0.span > $1.span }
    let (u, v) = (spans[0].axis, spans[1].axis)
    let points = corners.map { PlanePoint(x: Double($0[u]), z: Double($0[v])) }
    return (points, [spans[0].span, spans[1].span])
  }
}
