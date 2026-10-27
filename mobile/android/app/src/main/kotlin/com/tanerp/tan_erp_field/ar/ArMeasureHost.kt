package com.tanerp.tan_erp_field.ar

import android.app.Activity
import android.content.Intent
import android.os.Handler
import android.os.Looper
import androidx.activity.ComponentActivity
import androidx.activity.result.contract.ActivityResultContracts
import com.google.ar.core.ArCoreApk

/** Pigeon host side: checks ARCore availability and launches [ArMeasureActivity]. */
class ArMeasureHost(private val activity: ComponentActivity) : ArMeasureApi {
    private var pending: ((Result<ArMeasureResult?>) -> Unit)? = null

    private val launcher =
        activity.registerForActivityResult(ActivityResultContracts.StartActivityForResult()) { result ->
            val callback = pending ?: return@registerForActivityResult
            pending = null
            val data = result.data
            val error = data?.getStringExtra(ArMeasureActivity.EXTRA_ERROR)
            when {
                error != null -> callback(Result.failure(FlutterError(error)))
                result.resultCode == Activity.RESULT_OK && data != null -> {
                    val edges = data.getDoubleArrayExtra(ArMeasureActivity.EXTRA_EDGES) ?: DoubleArray(0)
                    callback(
                        Result.success(
                            ArMeasureResult(
                                areaSqm = data.getDoubleExtra(ArMeasureActivity.EXTRA_AREA, 0.0),
                                perimeterM = data.getDoubleExtra(ArMeasureActivity.EXTRA_PERIMETER, 0.0),
                                edgeLengthsM = edges.toList(),
                                vertexCount = edges.size.toLong(),
                            ),
                        ),
                    )
                }
                else -> callback(Result.success(null))
            }
        }

    override fun checkAvailability(callback: (Result<ArAvailability>) -> Unit) {
        poll(callback, attemptsLeft = CHECK_ATTEMPTS)
    }

    // ARCore answers UNKNOWN_CHECKING while it queries its service; retry briefly.
    private fun poll(callback: (Result<ArAvailability>) -> Unit, attemptsLeft: Int) {
        val availability = ArCoreApk.getInstance().checkAvailability(activity)
        if (availability.isTransient && attemptsLeft > 0) {
            Handler(Looper.getMainLooper()).postDelayed({ poll(callback, attemptsLeft - 1) }, CHECK_DELAY_MS)
            return
        }
        callback(Result.success(map(availability)))
    }

    private fun map(availability: ArCoreApk.Availability): ArAvailability = when (availability) {
        ArCoreApk.Availability.SUPPORTED_INSTALLED -> ArAvailability.SUPPORTED
        ArCoreApk.Availability.SUPPORTED_APK_TOO_OLD,
        ArCoreApk.Availability.SUPPORTED_NOT_INSTALLED -> ArAvailability.NEEDS_INSTALL
        ArCoreApk.Availability.UNSUPPORTED_DEVICE_NOT_CAPABLE -> ArAvailability.UNSUPPORTED
        else -> ArAvailability.UNKNOWN
    }

    override fun measureFloor(request: ArMeasureRequest, callback: (Result<ArMeasureResult?>) -> Unit) {
        if (pending != null) {
            callback(Result.failure(FlutterError(ArMeasureActivity.ERROR_BUSY)))
            return
        }
        pending = callback
        ArMeasureActivity.request = request
        launcher.launch(Intent(activity, ArMeasureActivity::class.java))
    }

    private companion object {
        const val CHECK_ATTEMPTS = 15
        const val CHECK_DELAY_MS = 200L
    }
}
