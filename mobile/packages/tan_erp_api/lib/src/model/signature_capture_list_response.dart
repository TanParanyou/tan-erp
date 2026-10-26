//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/signature_capture_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'signature_capture_list_response.g.dart';

/// SignatureCaptureListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class SignatureCaptureListResponse implements Built<SignatureCaptureListResponse, SignatureCaptureListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SignatureCaptureResponse>? get items;

  SignatureCaptureListResponse._();

  factory SignatureCaptureListResponse([void updates(SignatureCaptureListResponseBuilder b)]) = _$SignatureCaptureListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SignatureCaptureListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SignatureCaptureListResponse> get serializer => _$SignatureCaptureListResponseSerializer();
}

class _$SignatureCaptureListResponseSerializer implements PrimitiveSerializer<SignatureCaptureListResponse> {
  @override
  final Iterable<Type> types = const [SignatureCaptureListResponse, _$SignatureCaptureListResponse];

  @override
  final String wireName = r'SignatureCaptureListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SignatureCaptureListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SignatureCaptureResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SignatureCaptureListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SignatureCaptureListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SignatureCaptureResponse)]),
          ) as BuiltList<SignatureCaptureResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SignatureCaptureListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SignatureCaptureListResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


