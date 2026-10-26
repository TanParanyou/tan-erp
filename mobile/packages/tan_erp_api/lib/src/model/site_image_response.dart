//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_image_response.g.dart';

/// SiteImageResponse
///
/// Properties:
/// * [id] 
/// * [fileId] 
/// * [caption] 
/// * [displayOrder] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class SiteImageResponse implements Built<SiteImageResponse, SiteImageResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  @BuiltValueField(wireName: r'displayOrder')
  int? get displayOrder;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  SiteImageResponse._();

  factory SiteImageResponse([void updates(SiteImageResponseBuilder b)]) = _$SiteImageResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteImageResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteImageResponse> get serializer => _$SiteImageResponseSerializer();
}

class _$SiteImageResponseSerializer implements PrimitiveSerializer<SiteImageResponse> {
  @override
  final Iterable<Type> types = const [SiteImageResponse, _$SiteImageResponse];

  @override
  final String wireName = r'SiteImageResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayOrder != null) {
      yield r'displayOrder';
      yield serializers.serialize(
        object.displayOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteImageResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.caption = valueDes;
          break;
        case r'displayOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.displayOrder = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteImageResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteImageResponseBuilder();
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


