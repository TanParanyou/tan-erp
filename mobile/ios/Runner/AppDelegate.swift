import Flutter
import UIKit

@main
@objc class AppDelegate: FlutterAppDelegate, FlutterImplicitEngineDelegate {
  // One host each for the app lifetime; they hold the pending native-screen callback.
  private let arMeasureHost = ArMeasureHost()
  private let roomScanHost = RoomScanHost()

  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }

  func didInitializeImplicitFlutterEngine(_ engineBridge: FlutterImplicitEngineBridge) {
    GeneratedPluginRegistrant.register(with: engineBridge.pluginRegistry)
    if let registrar = engineBridge.pluginRegistry.registrar(forPlugin: "ArMeasureApi") {
      ArMeasureApiSetup.setUp(binaryMessenger: registrar.messenger(), api: arMeasureHost)
      RoomScanApiSetup.setUp(binaryMessenger: registrar.messenger(), api: roomScanHost)
    }
  }
}
