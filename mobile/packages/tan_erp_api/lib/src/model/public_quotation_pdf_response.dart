//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_quotation_pdf_response.g.dart';

/// PublicQuotationPdfResponse
///
/// Properties:
/// * [sha256] 
/// * [byteCount] 
@BuiltValue()
abstract class PublicQuotationPdfResponse implements Built<PublicQuotationPdfResponse, PublicQuotationPdfResponseBuilder> {
  @BuiltValueField(wireName: r'sha256')
  String? get sha256;

  @BuiltValueField(wireName: r'byteCount')
  int? get byteCount;

  PublicQuotationPdfResponse._();

  factory PublicQuotationPdfResponse([void updates(PublicQuotationPdfResponseBuilder b)]) = _$PublicQuotationPdfResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicQuotationPdfResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicQuotationPdfResponse> get serializer => _$PublicQuotationPdfResponseSerializer();
}

class _$PublicQuotationPdfResponseSerializer implements PrimitiveSerializer<PublicQuotationPdfResponse> {
  @override
  final Iterable<Type> types = const [PublicQuotationPdfResponse, _$PublicQuotationPdfResponse];

  @override
  final String wireName = r'PublicQuotationPdfResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicQuotationPdfResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sha256 != null) {
      yield r'sha256';
      yield serializers.serialize(
        object.sha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.byteCount != null) {
      yield r'byteCount';
      yield serializers.serialize(
        object.byteCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicQuotationPdfResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicQuotationPdfResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sha256 = valueDes;
          break;
        case r'byteCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.byteCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicQuotationPdfResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicQuotationPdfResponseBuilder();
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


