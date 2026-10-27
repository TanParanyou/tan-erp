import Flutter
import UIKit

@main
@objc class AppDelegate: FlutterAppDelegate, FlutterImplicitEngineDelegate {
  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }

  func didInitializeImplicitFlutterEngine(_ engineBridge: FlutterImplicitEngineBridge) {
    GeneratedPluginRegistrant.register(with: engineBridge.pluginRegistry)
    if let registrar = engineBridge.pluginRegistry.registrar(forPlugin: "ArMeasureApi") {
      ArMeasureApiSetup.setUp(binaryMessenger: registrar.messenger(), api: ArMeasureStub())
    }
  }
}

/// iOS AR measure is paused: always reports unsupported (Device Lab, Android only).
private final class ArMeasureStub: ArMeasureApi {
  func checkAvailability(completion: @escaping (Result<ArAvailability, Error>) -> Void) {
    completion(.success(.unsupported))
  }

  func measureFloor(
    request: ArMeasureRequest, completion: @escaping (Result<ArMeasureResult?, Error>) -> Void
  ) {
    completion(.failure(PigeonError(code: "unavailable", message: nil, details: nil)))
  }
}
