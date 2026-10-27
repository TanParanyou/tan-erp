package com.tanerp.tan_erp_field

import com.tanerp.tan_erp_field.ar.ArMeasureApi
import com.tanerp.tan_erp_field.ar.ArMeasureHost
import io.flutter.embedding.android.FlutterFragmentActivity
import io.flutter.embedding.engine.FlutterEngine

class MainActivity : FlutterFragmentActivity() {
    // Registered at construction: activity-result launchers must exist before onStart.
    private val arMeasure = ArMeasureHost(this)

    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)
        ArMeasureApi.setUp(flutterEngine.dartExecutor.binaryMessenger, arMeasure)
    }
}
