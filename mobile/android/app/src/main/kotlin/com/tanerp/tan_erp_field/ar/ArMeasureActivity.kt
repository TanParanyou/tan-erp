package com.tanerp.tan_erp_field.ar

import android.Manifest
import android.app.Activity
import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.PointF
import android.graphics.drawable.GradientDrawable
import android.opengl.GLES20
import android.opengl.GLSurfaceView
import android.opengl.Matrix
import android.os.Build
import android.os.Bundle
import android.os.SystemClock
import android.util.TypedValue
import android.view.Gravity
import android.view.View
import android.view.ViewGroup.LayoutParams.MATCH_PARENT
import android.view.ViewGroup.LayoutParams.WRAP_CONTENT
import android.widget.FrameLayout
import android.widget.LinearLayout
import android.widget.TextView
import com.google.ar.core.Anchor
import com.google.ar.core.ArCoreApk
import com.google.ar.core.Camera
import com.google.ar.core.Config
import com.google.ar.core.Frame
import com.google.ar.core.Plane
import com.google.ar.core.Session
import com.google.ar.core.TrackingState
import com.google.ar.core.exceptions.CameraNotAvailableException
import com.google.ar.core.exceptions.UnavailableException
import com.google.ar.core.exceptions.UnavailableUserDeclinedInstallationException
import java.util.Locale
import java.util.concurrent.ConcurrentLinkedQueue
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

/**
 * Native AR floor measure: find a horizontal plane, tap to place corners,
 * close the polygon and return area/perimeter/edges. Texts and colours come
 * from the Flutter request ([request]); this screen defines none of its own.
 */
class ArMeasureActivity : Activity(), GLSurfaceView.Renderer {
    private lateinit var strings: ArMeasureStrings
    private lateinit var colors: ArMeasureColors
    private lateinit var locale: Locale

    private lateinit var surfaceView: GLSurfaceView
    private lateinit var overlay: MeasureOverlayView
    private lateinit var areaText: TextView
    private lateinit var messageText: TextView
    private lateinit var undoButton: TextView
    private lateinit var closeButton: TextView
    private lateinit var doneButton: TextView

    private var session: Session? = null
    private var installRequested = false
    private var permissionRequested = false
    private val background = BackgroundRenderer()
    private var textureBound = false
    private var viewportChanged = false
    private var viewWidth = 0
    private var viewHeight = 0

    // GL-thread state.
    private val taps = ConcurrentLinkedQueue<PointF>()
    private val anchors = mutableListOf<Anchor>()
    private var referencePlane: Plane? = null
    @Volatile private var closed = false
    private var everTracked = false
    private var transientMessage: String? = null
    private var transientUntil = 0L
    private val viewMatrix = FloatArray(16)
    private val projectionMatrix = FloatArray(16)
    private val viewProjection = FloatArray(16)

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val req = request
        if (req == null) {
            finishWithError(ERROR_SESSION_FAILED)
            return
        }
        strings = req.strings
        colors = req.colors
        locale = Locale.forLanguageTag(strings.localeTag)
        buildViews()
    }

    override fun onResume() {
        super.onResume()
        if (!::surfaceView.isInitialized) return
        if (session == null && !createSession()) return
        try {
            session?.resume()
        } catch (e: CameraNotAvailableException) {
            finishWithError(ERROR_SESSION_FAILED)
            return
        }
        surfaceView.onResume()
    }

    override fun onPause() {
        super.onPause()
        if (!::surfaceView.isInitialized) return
        surfaceView.onPause()
        session?.pause()
    }

    override fun onDestroy() {
        // Closing the session releases the camera and detaches every anchor.
        session?.close()
        session = null
        if (isFinishing) request = null
        super.onDestroy()
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == CAMERA_REQUEST && !hasCameraPermission()) finishWithError(ERROR_CAMERA_PERMISSION)
        // When granted, onResume runs again and creates the session.
    }

    /** Returns false while waiting for ARCore install or the camera permission. */
    private fun createSession(): Boolean {
        try {
            when (ArCoreApk.getInstance().requestInstall(this, !installRequested)) {
                ArCoreApk.InstallStatus.INSTALL_REQUESTED -> {
                    installRequested = true
                    return false
                }
                ArCoreApk.InstallStatus.INSTALLED -> Unit
                null -> return false
            }
            // Ask after ARCore is installed so the two system dialogs never overlap.
            if (!hasCameraPermission()) {
                if (!permissionRequested) {
                    permissionRequested = true
                    requestPermissions(arrayOf(Manifest.permission.CAMERA), CAMERA_REQUEST)
                }
                return false
            }
            val s = Session(this)
            s.configure(
                Config(s).apply {
                    planeFindingMode = Config.PlaneFindingMode.HORIZONTAL
                    updateMode = Config.UpdateMode.LATEST_CAMERA_IMAGE
                    focusMode = Config.FocusMode.AUTO
                },
            )
            session = s
            textureBound = false
            return true
        } catch (e: UnavailableUserDeclinedInstallationException) {
            finishWithError(ERROR_INSTALL_DECLINED)
        } catch (e: UnavailableException) {
            finishWithError(ERROR_UNAVAILABLE)
        } catch (e: RuntimeException) {
            finishWithError(ERROR_SESSION_FAILED)
        }
        return false
    }

    private fun hasCameraPermission() =
        checkSelfPermission(Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED

    private fun finishWithError(code: String) {
        setResult(RESULT_CANCELED, Intent().putExtra(EXTRA_ERROR, code))
        finish()
    }

    // ---- Views ----------------------------------------------------------

    private fun dp(value: Float) = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, value, resources.displayMetrics)

    private fun buildViews() {
        val space = dp(SPACE_DP).toInt()
        surfaceView = GLSurfaceView(this).apply {
            preserveEGLContextOnPause = true
            setEGLContextClientVersion(2)
            setRenderer(this@ArMeasureActivity)
            renderMode = GLSurfaceView.RENDERMODE_CONTINUOUSLY
        }
        overlay = MeasureOverlayView(this, colors) { x, y -> if (!closed) taps.add(PointF(x, y)) }

        val cancel = button(strings.cancel, primary = false).apply {
            setOnClickListener { finish() }
        }

        areaText = TextView(this).apply {
            setTextColor(colors.textMain.toInt())
            setTextSize(TypedValue.COMPLEX_UNIT_SP, AREA_SP)
            typeface = android.graphics.Typeface.DEFAULT_BOLD
            fontFeatureSettings = "tnum"
        }
        messageText = TextView(this).apply { setTextSize(TypedValue.COMPLEX_UNIT_SP, BODY_SP) }
        undoButton = button(strings.undo, primary = false).apply { setOnClickListener { surfaceView.queueEvent(::undo) } }
        closeButton = button(strings.closeShape, primary = false).apply {
            setOnClickListener { surfaceView.queueEvent(::closeShape) }
        }
        doneButton = button(strings.done, primary = true).apply { setOnClickListener { surfaceView.queueEvent(::complete) } }

        val row = LinearLayout(this).apply {
            orientation = LinearLayout.HORIZONTAL
            addView(undoButton, LinearLayout.LayoutParams(0, WRAP_CONTENT, 1f).apply { marginEnd = space / 2 })
            addView(closeButton, LinearLayout.LayoutParams(0, WRAP_CONTENT, 1f).apply { marginStart = space / 2 })
        }
        val panel = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            setBackgroundColor(colors.surface.toInt())
            setPadding(space, space, space, space)
            addView(label(strings.areaLabel))
            addView(areaText)
            addView(messageText, LinearLayout.LayoutParams(MATCH_PARENT, WRAP_CONTENT).apply { bottomMargin = space })
            addView(row, LinearLayout.LayoutParams(MATCH_PARENT, WRAP_CONTENT).apply { bottomMargin = space })
            addView(doneButton, LinearLayout.LayoutParams(MATCH_PARENT, dp(PRIMARY_DP).toInt()))
        }

        val root = FrameLayout(this).apply {
            addView(surfaceView, FrameLayout.LayoutParams(MATCH_PARENT, MATCH_PARENT))
            addView(this@ArMeasureActivity.overlay, FrameLayout.LayoutParams(MATCH_PARENT, MATCH_PARENT))
            addView(cancel, FrameLayout.LayoutParams(WRAP_CONTENT, WRAP_CONTENT, Gravity.TOP or Gravity.START).apply {
                setMargins(space, space, space, space)
            })
            addView(panel, FrameLayout.LayoutParams(MATCH_PARENT, WRAP_CONTENT, Gravity.BOTTOM))
        }
        setContentView(root)
        render(UiState(strings.searchingPlane, isError = false, area = null, canUndo = false, canClose = false, canFinish = false))
    }

    private fun label(text: String) = TextView(this).apply {
        this.text = text
        setTextColor(colors.textMuted.toInt())
        setTextSize(TypedValue.COMPLEX_UNIT_SP, CAPTION_SP)
    }

    private fun button(text: String, primary: Boolean) = TextView(this).apply {
        this.text = text
        gravity = Gravity.CENTER
        minHeight = dp(TAP_DP).toInt()
        minWidth = dp(TAP_DP).toInt()
        val pad = dp(SPACE_DP).toInt()
        setPadding(pad, 0, pad, 0)
        setTextSize(TypedValue.COMPLEX_UNIT_SP, BODY_SP)
        isClickable = true
        tag = primary
        styleButton(this, enabled = true)
    }

    // Square corners: GradientDrawable defaults to radius 0.
    private fun styleButton(view: TextView, enabled: Boolean) {
        val primary = view.tag == true
        // Local copy: GradientDrawable.colors would shadow the field inside apply.
        val c = colors
        view.isEnabled = enabled
        view.background = GradientDrawable().apply {
            when {
                !enabled -> setColor(c.disabledBackground.toInt())
                primary -> setColor(c.action.toInt())
                else -> {
                    setColor(c.surface.toInt())
                    setStroke(dp(1f).toInt().coerceAtLeast(1), c.borderStrong.toInt())
                }
            }
        }
        view.setTextColor(
            when {
                !enabled -> colors.disabledForeground
                primary -> colors.onAction
                else -> colors.textMain
            }.toInt(),
        )
    }

    private class UiState(
        val message: String,
        val isError: Boolean,
        val area: Double?,
        val canUndo: Boolean,
        val canClose: Boolean,
        val canFinish: Boolean,
    )

    private fun render(ui: UiState) {
        areaText.text = format(ui.area ?: 0.0, strings.unitSquareMeter)
        messageText.text = ui.message
        messageText.setTextColor((if (ui.isError) colors.danger else colors.textMain).toInt())
        if (undoButton.isEnabled != ui.canUndo) styleButton(undoButton, ui.canUndo)
        if (closeButton.isEnabled != ui.canClose) styleButton(closeButton, ui.canClose)
        if (doneButton.isEnabled != ui.canFinish) styleButton(doneButton, ui.canFinish)
    }

    private fun format(value: Double, unit: String) = String.format(locale, "%,.2f %s", value, unit)

    // ---- GL thread --------------------------------------------------------

    override fun onSurfaceCreated(gl: GL10?, config: EGLConfig?) {
        background.create()
        textureBound = false
    }

    override fun onSurfaceChanged(gl: GL10?, width: Int, height: Int) {
        GLES20.glViewport(0, 0, width, height)
        viewWidth = width
        viewHeight = height
        viewportChanged = true
    }

    override fun onDrawFrame(gl: GL10?) {
        GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT or GLES20.GL_DEPTH_BUFFER_BIT)
        val s = session ?: return
        if (!textureBound) {
            s.setCameraTextureName(background.textureId)
            textureBound = true
        }
        if (viewportChanged) {
            s.setDisplayGeometry(displayRotation(), viewWidth, viewHeight)
            viewportChanged = false
        }
        val frame = try {
            s.update()
        } catch (e: Exception) {
            return // paused or camera lost; the next resume restarts updates
        }
        background.draw(frame)
        val camera = frame.camera
        val tracking = camera.trackingState == TrackingState.TRACKING
        if (tracking) everTracked = true
        while (true) {
            val tap = taps.poll() ?: break
            if (tracking) placePoint(frame, tap)
        }
        publish(s, frame, camera, tracking)
    }

    @Suppress("DEPRECATION")
    private fun displayRotation(): Int =
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) display!!.rotation else windowManager.defaultDisplay.rotation

    private fun rootPlane(plane: Plane): Plane = plane.subsumedBy?.let(::rootPlane) ?: plane

    private fun horizontalHit(frame: Frame, x: Float, y: Float) = frame.hitTest(x, y).firstOrNull { hit ->
        val plane = hit.trackable as? Plane
        plane != null &&
            plane.type == Plane.Type.HORIZONTAL_UPWARD_FACING &&
            plane.isPoseInPolygon(hit.hitPose) &&
            (referencePlane == null || rootPlane(plane) == rootPlane(referencePlane!!))
    }

    private fun placePoint(frame: Frame, tap: PointF) {
        val hit = horizontalHit(frame, tap.x, tap.y)
        if (hit == null) {
            showTransient(strings.notOnPlane)
            return
        }
        if (referencePlane == null) referencePlane = hit.trackable as Plane
        anchors.add(hit.createAnchor())
    }

    private fun undo() {
        when {
            closed -> closed = false
            anchors.isNotEmpty() -> anchors.removeAt(anchors.lastIndex).detach()
        }
        if (anchors.isEmpty()) referencePlane = null
    }

    private fun closeShape() {
        if (closed || anchors.size < 3) return
        if (PolygonMath.isSelfIntersecting(planePoints())) {
            showTransient(strings.selfIntersecting)
            return
        }
        closed = true
    }

    private fun complete() {
        if (!closed) return
        val points = planePoints()
        val edges = PolygonMath.edgeLengths(points, closed = true)
        val data = Intent()
            .putExtra(EXTRA_AREA, PolygonMath.area(points))
            .putExtra(EXTRA_PERIMETER, edges.sum())
            .putExtra(EXTRA_EDGES, edges.toDoubleArray())
        runOnUiThread {
            setResult(RESULT_OK, data)
            finish()
        }
    }

    private fun showTransient(message: String) {
        transientMessage = message
        transientUntil = SystemClock.uptimeMillis() + TRANSIENT_MS
    }

    /** Corner positions in the reference plane's local frame (x, z). */
    private fun planePoints(): List<PlanePoint> {
        val inverse = rootPlane(referencePlane ?: return emptyList()).centerPose.inverse()
        return anchors.map { anchor ->
            val local = inverse.transformPoint(anchor.pose.translation)
            PlanePoint(local[0].toDouble(), local[2].toDouble())
        }
    }

    private fun project(world: FloatArray): PointF? {
        val clip = FloatArray(4)
        Matrix.multiplyMV(clip, 0, viewProjection, 0, floatArrayOf(world[0], world[1], world[2], 1f), 0)
        if (clip[3] <= 0f) return null
        val x = (clip[0] / clip[3] + 1f) / 2f * viewWidth
        val y = (1f - clip[1] / clip[3]) / 2f * viewHeight
        return PointF(x, y)
    }

    private fun publish(s: Session, frame: Frame, camera: Camera, tracking: Boolean) {
        camera.getViewMatrix(viewMatrix, 0)
        camera.getProjectionMatrix(projectionMatrix, 0, NEAR, FAR)
        Matrix.multiplyMM(viewProjection, 0, projectionMatrix, 0, viewMatrix, 0)

        val screen = anchors.map { project(it.pose.translation) }
        val points = planePoints()
        val edges = PolygonMath.edgeLengths(points, closed)
        val labels = edges.mapIndexedNotNull { i, length ->
            val a = screen[i] ?: return@mapIndexedNotNull null
            val b = screen[(i + 1) % screen.size] ?: return@mapIndexedNotNull null
            EdgeLabel(format(length, strings.unitMeter), PointF((a.x + b.x) / 2f, (a.y + b.y) / 2f))
        }
        val planeFound = tracking && s.getAllTrackables(Plane::class.java).any {
            it.trackingState == TrackingState.TRACKING && it.type == Plane.Type.HORIZONTAL_UPWARD_FACING
        }
        val reticleOnPlane = tracking && horizontalHit(frame, viewWidth / 2f, viewHeight / 2f) != null

        val transient = transientMessage?.takeIf { SystemClock.uptimeMillis() < transientUntil }
        val (message, isError) = when {
            !tracking && everTracked -> strings.trackingLost to true
            transient != null -> transient to true
            !planeFound && anchors.isEmpty() -> strings.searchingPlane to false
            anchors.isEmpty() -> strings.tapToPlace to false
            anchors.size < 3 -> strings.addMorePoints to false
            !closed -> strings.readyToClose to false
            else -> strings.closedHint to false
        }
        val ui = UiState(
            message = message,
            isError = isError,
            area = if (points.size >= 3) PolygonMath.area(points) else null,
            canUndo = anchors.isNotEmpty(),
            canClose = !closed && anchors.size >= 3,
            canFinish = closed,
        )
        val overlayState = OverlayState(screen, closed, labels, reticleOnPlane)
        runOnUiThread {
            overlay.state = overlayState
            overlay.invalidate()
            render(ui)
        }
    }

    companion object {
        /** Request handed over by [ArMeasureHost]; shortcut: lost on process death, then the screen reports session_failed. */
        @Volatile
        var request: ArMeasureRequest? = null

        const val EXTRA_ERROR = "error"
        const val EXTRA_AREA = "areaSqm"
        const val EXTRA_PERIMETER = "perimeterM"
        const val EXTRA_EDGES = "edgeLengthsM"

        const val ERROR_CAMERA_PERMISSION = "camera_permission_denied"
        const val ERROR_INSTALL_DECLINED = "install_declined"
        const val ERROR_UNAVAILABLE = "unavailable"
        const val ERROR_SESSION_FAILED = "session_failed"
        const val ERROR_BUSY = "busy"

        private const val CAMERA_REQUEST = 1
        private const val TRANSIENT_MS = 2500L
        private const val NEAR = 0.1f
        private const val FAR = 100f
        private const val SPACE_DP = 16f
        private const val TAP_DP = 48f
        private const val PRIMARY_DP = 52f
        private const val CAPTION_SP = 13f
        private const val BODY_SP = 15f
        private const val AREA_SP = 44f
    }
}
